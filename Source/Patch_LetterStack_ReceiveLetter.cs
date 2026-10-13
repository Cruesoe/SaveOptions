using System;
using System.Diagnostics;
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
            (let.def != LetterDefOf.ThreatBig && let.def != LetterDefOf.NegativeEvent))
        {
            return;
        }

        // Ignore injury letters sent inside damage application without patching another mod's assembly.
        if (AccessTools.TypeByName("LetterPermanentInjury.Patch") != null)
        {
            foreach (StackFrame frame in new StackTrace().GetFrames())
            {
                if (frame.GetMethod()?.DeclaringType?.FullName == "LetterPermanentInjury.Patch")
                {
                    return;
                }
            }
        }
        Current.Game?.GetComponent<NegativeEventSaver>()?.RequestSave(let);
    }
}
