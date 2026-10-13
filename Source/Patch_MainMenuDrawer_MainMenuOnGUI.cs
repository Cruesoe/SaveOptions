using HarmonyLib;
using RimWorld;
using Verse;

namespace SaveOptions;

[HarmonyPatch(typeof(MainMenuDrawer), nameof(MainMenuDrawer.MainMenuOnGUI))]
public static class Patch_MainMenuDrawer_MainMenuOnGUI
{
    public static void Prefix()
    {
        if (Current.ProgramState == ProgramState.Entry)
        {
            AutoLoadLastSave.ObserveInput();
        }
    }

    public static void Postfix()
    {
        if (Current.ProgramState == ProgramState.Entry && !LongEventHandler.ShouldWaitForEvent)
        {
            AutoLoadLastSave.OnMainMenu();
        }
    }
}
