using System;
using HarmonyLib;
using Verse;

namespace SaveOptions;

// Loads the Save and Quit mod's game components from older saves as their Save Options equivalents.
[HarmonyPatch(typeof(BackCompatibility), nameof(BackCompatibility.GetBackCompatibleType))]
public static class Patch_BackCompatibility_GetBackCompatibleType
{
    public static void Postfix(string providedClassName, ref Type? __result)
    {
        if (__result != null)
        {
            return;
        }

        switch (providedClassName)
        {
            case "SaveAndQuit.SaveNameTracker":
                __result = typeof(SaveNameTracker);
                break;
            case "SaveAndQuit.GameStartSaver":
                __result = typeof(GameStartSaver);
                break;
        }
    }
}
