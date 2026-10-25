using HarmonyLib;
using UnityEngine;
using Verse;

namespace SaveOptions;

public sealed class SaveOptionsMod : Mod
{
    public static SaveOptionsSettings Settings = null!;

    public SaveOptionsMod(ModContentPack content) : base(content)
    {
        Settings = GetSettings<SaveOptionsSettings>();
        new Harmony("cruesoe.saveoptions").PatchAll();
    }

    public override string SettingsCategory() => "SaveOptions_SettingsCategory".Translate();

    public override void DoSettingsWindowContents(Rect inRect)
    {
        Text.Font = GameFont.Small;
        Listing_Standard list = new();
        list.Begin(inRect);
        list.CheckboxLabeled("SaveOptions_NegativeEvents".Translate(), ref Settings.SaveOnNegativeEvent,
            "SaveOptions_NegativeEventsTip".Translate());
        bool previousEnabled = GUI.enabled;
        GUI.enabled = previousEnabled && Settings.SaveOnNegativeEvent;
        list.CheckboxLabeled("SaveOptions_AppendEventLabel".Translate(), ref Settings.AppendEventLabel,
            "SaveOptions_AppendEventLabelTip".Translate());
        list.Label("SaveOptions_Cooldown".Translate(Settings.CooldownSeconds), tooltip: "SaveOptions_NegativeEventsRequired".Translate());
        Settings.CooldownSeconds = (int)list.Slider(Settings.CooldownSeconds, 1, 200);
        GUI.enabled = previousEnabled;
        list.GapLine();
        list.CheckboxLabeled("SaveOptions_SaveAndQuit".Translate(), ref Settings.SaveAndQuit,
            "SaveOptions_SaveAndQuitTip".Translate());
        list.CheckboxLabeled("SaveOptions_SaveOnGameStart".Translate(), ref Settings.SaveOnGameStart,
            "SaveOptions_SaveOnGameStartTip".Translate());
        list.CheckboxLabeled("SaveOptions_QuickSaveKeys".Translate(), ref Settings.QuickSaveKeys,
            "SaveOptions_QuickSaveKeysTip".Translate());
        list.GapLine();
        list.CheckboxLabeled("SaveOptions_AutoLoad".Translate(), ref Settings.AutoLoadLastSave,
            "SaveOptions_AutoLoadTip".Translate());
        GUI.enabled = previousEnabled && Settings.AutoLoadLastSave;
        list.CheckboxLabeled("SaveOptions_ShowPrompt".Translate(), ref Settings.ShowAutoLoadPrompt,
            "SaveOptions_ShowPromptTip".Translate());
        GUI.enabled = previousEnabled && Settings.AutoLoadLastSave && Settings.ShowAutoLoadPrompt;
        list.Label("SaveOptions_PromptDuration".Translate(Settings.PromptSeconds), tooltip: "SaveOptions_PromptRequired".Translate());
        Settings.PromptSeconds = (int)list.Slider(Settings.PromptSeconds, 1, 60);
        GUI.enabled = previousEnabled && Settings.AutoLoadLastSave;
        list.CheckboxLabeled("SaveOptions_Logging".Translate(), ref Settings.LoggingEnabled,
            "SaveOptions_LoggingTip".Translate());
        GUI.enabled = previousEnabled;
        list.End();
    }
}
