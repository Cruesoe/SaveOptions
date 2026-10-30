using System.IO;
using RimWorld;
using UnityEngine;
using Verse;

namespace SaveOptions;

/// <summary>Ctrl+S saves to "Quicksave" and Ctrl+L loads it.</summary>
public sealed class QuickSaveHotkeys : GameComponent
{
    public const string QuickSaveName = "Quicksave";

    private readonly Game game;
    private bool queued;

    public QuickSaveHotkeys(Game game) => this.game = game;

    // Runs before map gizmos, so using the event keeps Ctrl+L from also pressing a gizmo's L hotkey
    public override void GameComponentOnGUI()
    {
        Event current = Event.current;
        if (!SaveOptionsMod.Settings.QuickSaveKeys || current.type != EventType.KeyDown || !(current.control || current.command)
            || (current.keyCode != KeyCode.S && current.keyCode != KeyCode.L)
            || GUIUtility.keyboardControl != 0 || Find.WindowStack.IsOpen<Dialog_DefineBinding>())
        {
            return;
        }

        bool save = current.keyCode == KeyCode.S;
        current.Use();
        // Colony Groups opens its Slave menu from WorldComponentUpdate on any event with Ctrl and S, used or not
        current.keyCode = KeyCode.None;
        // Commitment mode keeps a single save
        if (game.Info.permadeathMode)
        {
            Messages.Message("SaveOptions_QuickSaveCommitment".Translate(), MessageTypeDefOf.RejectInput, false);
            return;
        }

        if (save)
        {
            QuickSave();
        }
        else
        {
            QuickLoad();
        }
    }

    private void QuickSave()
    {
        if (queued)
        {
            return;
        }

        queued = true;
        LongEventHandler.QueueLongEvent(() =>
        {
            try
            {
                if (SaveService.TrySave(game, QuickSaveName))
                {
                    Messages.Message("SaveOptions_QuickSaved".Translate(QuickSaveName), MessageTypeDefOf.SilentInput, false);
                }
            }
            finally
            {
                queued = false;
            }
        }, "SavingLongEvent", false, null);
    }

    private static void QuickLoad()
    {
        if (!File.Exists(GenFilePaths.FilePathForSavedGame(QuickSaveName)))
        {
            Messages.Message("SaveOptions_NoQuickSave".Translate(QuickSaveName), MessageTypeDefOf.RejectInput, false);
            return;
        }

        GameDataSaveLoader.CheckVersionAndLoadGame(QuickSaveName);
    }
}
