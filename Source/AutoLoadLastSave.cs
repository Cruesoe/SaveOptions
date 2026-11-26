using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

namespace SaveOptions;

public static class AutoLoadLastSave
{
    private static bool checkedStartup;
    private static bool skipped;
    private static float? promptEndsAt;
    private static bool reportedWindows;

    public static void ObserveInput()
    {
        EventType type = Event.current.type;
        if (type != EventType.KeyDown && type != EventType.MouseDown)
        {
            return;
        }
        if (!skipped && SaveOptionsMod.Settings.AutoLoadLastSave)
        {
            string input = type == EventType.KeyDown ? $"a key press ({Event.current.keyCode})" : "a mouse click";
            string stage = promptEndsAt == null ? "before the countdown started" : "during the countdown";
            Log.Message($"[Save Options] Automatic load cancelled by {input} {stage}. Open windows: {OpenWindows()}.");
        }
        skipped = true;
    }

    private static string OpenWindows()
    {
        if (Find.WindowStack == null || Find.WindowStack.Windows.Count == 0)
        {
            return "none";
        }
        return string.Join(", ", Find.WindowStack.Windows.Select(window =>
            window is ImmediateWindow ? $"ImmediateWindow {window.ID}" : window.GetType().FullName));
    }

    public static void OnMainMenu()
    {
        SaveOptionsSettings settings = SaveOptionsMod.Settings;
        if (!checkedStartup)
        {
            checkedStartup = true;
            if (settings.AutoLoadInProgress)
            {
                skipped = true;
                settings.AutoLoadInProgress = false;
                settings.Write();
                Log.Warning("[Save Options] The previous automatic load did not finish; staying at the main menu.");
            }
            else if (settings.AutoLoadLastSave && Log.Messages.Any(message => message.type == LogMessageType.Error))
            {
                skipped = true;
                Log.Message("[Save Options] Errors were logged during startup; staying at the main menu instead of loading the latest save.");
            }
        }
        if (skipped)
        {
            return;
        }
        if (!settings.AutoLoadLastSave)
        {
            skipped = true;
            return;
        }
        if (Find.WindowStack.Windows.Count > 0)
        {
            if (!reportedWindows)
            {
                reportedWindows = true;
                Log.Message($"[Save Options] Waiting for these windows to close before the automatic load: {OpenWindows()}.");
            }
            return;
        }

        if (settings.ShowAutoLoadPrompt)
        {
            promptEndsAt ??= Time.realtimeSinceStartup + settings.PromptSeconds;
            if (Time.realtimeSinceStartup < promptEndsAt.Value)
            {
                DrawPrompt((int)Math.Ceiling(promptEndsAt.Value - Time.realtimeSinceStartup));
                return;
            }
        }

        skipped = true;
        try
        {
            FileInfo? latest = SavePolicies.LatestSave(GenFilePaths.AllSavedGameFiles);
            if (latest == null)
            {
                Log.Message("[Save Options] No save found to load automatically.");
                return;
            }
            if (ScribeMetaHeaderUtility.GameVersionOf(latest).NullOrEmpty())
            {
                Log.Message($"[Save Options] '{latest.Name}' has no readable game version; staying at the main menu.");
                return;
            }
            if (settings.LoggingEnabled)
            {
                Log.Message($"[Save Options] Automatically loading '{latest.Name}'.");
            }
            // Same as GameDataSaveLoader.CheckVersionAndLoadGame; the flag is set only once the load starts, after any mismatch dialog.
            string saveName = Path.GetFileNameWithoutExtension(latest.Name);
            PreLoadUtility.CheckVersionAndLoad(GenFilePaths.FilePathForSavedGame(saveName), ScribeMetaHeaderUtility.ScribeHeaderMode.Map, () =>
            {
                settings.AutoLoadInProgress = true;
                settings.Write();
                GameDataSaveLoader.LoadGame(saveName);
            });
        }
        catch (Exception exception)
        {
            settings.AutoLoadInProgress = false;
            settings.Write();
            Log.Warning($"[Save Options] Could not automatically load the latest save: {exception}");
        }
    }

    private static void DrawPrompt(int seconds)
    {
        Rect rect = new((UI.screenWidth - 520f) / 2f, UI.screenHeight - 150f, 520f, 90f);
        Widgets.DrawWindowBackground(rect);
        GameFont previousFont = Text.Font;
        TextAnchor previousAnchor = Text.Anchor;
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(rect.ContractedBy(12f), "SaveOptions_AutoLoadPrompt".Translate(seconds));
        Text.Font = previousFont;
        Text.Anchor = previousAnchor;
    }

    public static void ClearPendingLoad()
    {
        skipped = true;
        if (SaveOptionsMod.Settings.AutoLoadInProgress)
        {
            SaveOptionsMod.Settings.AutoLoadInProgress = false;
            SaveOptionsMod.Settings.Write();
        }
    }
}
