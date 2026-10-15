using UnityEngine;
using Verse;

namespace SaveOptions;

public sealed class SaveOptionsSettings : ModSettings
{
    public bool SaveOnNegativeEvent = true;
    public bool AppendEventLabel;
    public int CooldownSeconds = 30;
    public bool SaveAndQuit = true;
    public bool SaveOnGameStart;
    public bool AutoLoadLastSave = true;
    public bool ShowAutoLoadPrompt = true;
    public int PromptSeconds = 3;
    public bool LoggingEnabled;
    public bool AutoLoadInProgress;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref SaveOnNegativeEvent, "saveOnNegativeEvent", true);
        Scribe_Values.Look(ref AppendEventLabel, "appendEventLabel", false);
        Scribe_Values.Look(ref CooldownSeconds, "cooldownSeconds", 30);
        Scribe_Values.Look(ref SaveAndQuit, "saveAndQuit", true);
        Scribe_Values.Look(ref SaveOnGameStart, "saveOnGameStart", false);
        Scribe_Values.Look(ref AutoLoadLastSave, "autoLoadLastSave", true);
        Scribe_Values.Look(ref ShowAutoLoadPrompt, "showAutoLoadPrompt", true);
        Scribe_Values.Look(ref PromptSeconds, "promptSeconds", 3);
        Scribe_Values.Look(ref LoggingEnabled, "loggingEnabled", false);
        Scribe_Values.Look(ref AutoLoadInProgress, "autoLoadInProgress", false);
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            CooldownSeconds = Mathf.Clamp(CooldownSeconds, 1, 200);
            PromptSeconds = Mathf.Clamp(PromptSeconds, 1, 60);
        }
    }
}
