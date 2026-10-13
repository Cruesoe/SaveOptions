using HarmonyLib;
using Verse;

namespace SaveOptions;

[HarmonyPatch(typeof(Root), nameof(Root.Shutdown))]
public static class Patch_Root_Shutdown
{
    public static void Prefix() => AutoLoadLastSave.ClearPendingLoad();
}
