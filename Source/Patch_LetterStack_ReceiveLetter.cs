using HarmonyLib;
using RimWorld;
using Verse;

namespace SaveOptions;

[HarmonyPatch(typeof(LetterStack), nameof(LetterStack.ReceiveLetter),
    new[] { typeof(Letter), typeof(string), typeof(int), typeof(bool) })]
public static class Patch_LetterStack_ReceiveLetter
{
    public static void Postfix(LetterStack __instance, Letter let, int delayTicks)
    {
        if (!SaveOptionsMod.Settings.SaveOnNegativeEvent || Current.ProgramState != ProgramState.Playing ||
            delayTicks > 0 || !__instance.LettersListForReading.Contains(let) ||
            (let.def != LetterDefOf.ThreatBig && let.def != LetterDefOf.NegativeEvent) ||
            Patch_DamageWorker_AddInjury_FinalizeAndAddInjury.AddingInjury)
        {
            return;
        }
        Current.Game?.GetComponent<NegativeEventSaver>()?.RequestSave(let);
    }
}
