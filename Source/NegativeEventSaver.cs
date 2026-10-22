using UnityEngine;
using RimWorld;
using Verse;

namespace SaveOptions;

public sealed class NegativeEventSaver : GameComponent
{
    private readonly Game game;
    private float lastSaveTime = float.NegativeInfinity;
    private string? pendingName;
    private bool queued;

    public NegativeEventSaver(Game game) => this.game = game;

    public void RequestSave(Letter letter)
    {
        SaveOptionsSettings settings = SaveOptionsMod.Settings;
        if (!settings.SaveOnNegativeEvent || pendingName != null || queued ||
            Time.realtimeSinceStartup - lastSaveTime < settings.CooldownSeconds)
        {
            return;
        }
        pendingName = SavePolicies.EventSaveName(letter.Label.RawText.StripTags(), settings.AppendEventLabel);
    }

    public override void GameComponentUpdate()
    {
        if (!SaveOptionsMod.Settings.SaveOnNegativeEvent)
        {
            pendingName = null;
            return;
        }
        if (pendingName == null || queued || GameDataSaveLoader.SavingIsTemporarilyDisabled)
        {
            return;
        }

        string fileName = pendingName;
        pendingName = null;
        queued = true;
        LongEventHandler.QueueLongEvent(() =>
        {
            try
            {
                if (SaveOptionsMod.Settings.SaveOnNegativeEvent && SaveService.TrySave(game, fileName))
                {
                    lastSaveTime = Time.realtimeSinceStartup;
                    Messages.Message("SaveOptions_SavedEventFile".Translate(fileName), MessageTypeDefOf.SilentInput, false);
                }
            }
            finally
            {
                queued = false;
            }
        }, "SavingLongEvent", false, null);
    }
}
