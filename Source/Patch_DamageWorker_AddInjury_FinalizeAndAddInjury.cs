using System;
using HarmonyLib;
using Verse;

namespace SaveOptions;

// Tracks injury finalization so letters sent from it (Letter Permanent Injury) don't trigger event saves.
[HarmonyPatch(typeof(DamageWorker_AddInjury), "FinalizeAndAddInjury",
    new[] { typeof(Pawn), typeof(Hediff_Injury), typeof(DamageInfo), typeof(DamageWorker.DamageResult) })]
public static class Patch_DamageWorker_AddInjury_FinalizeAndAddInjury
{
    private static int depth;

    public static bool AddingInjury => depth > 0;

    public static bool Prepare() => AccessTools.TypeByName("LetterPermanentInjury.Patch") != null;

    public static void Prefix() => depth++;

    public static void Finalizer() => depth--;
}
