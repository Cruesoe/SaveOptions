using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace SaveOptions;

[HarmonyPatch(typeof(OptionListingUtility), nameof(OptionListingUtility.DrawOptionListing))]
public static class Patch_OptionListingUtility_DrawOptionListing
{
    private static readonly Action SaveAndQuitToMainMenuAction = SaveAndQuitToMainMenu;
    private static readonly Action SaveAndQuitToOSAction = SaveAndQuitToOS;

    private static LoadedLanguage? labelsCachedFor;
    private static string quitToMainMenu = string.Empty;
    private static string quitToOS = string.Empty;
    private static string saveAndQuitToMainMenu = string.Empty;
    private static string saveAndQuitToOS = string.Empty;

    public static void Prefix(List<ListableOption> optList)
    {
        if (!ShouldReplaceQuitOptions())
        {
            return;
        }

        RefreshLabels();

        foreach (ListableOption option in optList)
        {
            if (option.label == quitToMainMenu)
            {
                option.label = saveAndQuitToMainMenu;
                option.action = SaveAndQuitToMainMenuAction;
            }
            else if (option.label == quitToOS)
            {
                option.label = saveAndQuitToOS;
                option.action = SaveAndQuitToOSAction;
            }
        }
    }

    private static bool ShouldReplaceQuitOptions()
    {
        if (!SaveOptionsMod.Settings.SaveAndQuit)
        {
            return false;
        }

        if (Current.ProgramState != ProgramState.Playing)
        {
            return false;
        }

        if (QuitWithoutSavingHeld())
        {
            return false;
        }

        Game game = Current.Game;
        if (game?.Info == null || game.Info.permadeathMode)
        {
            return false;
        }

        if (GameDataSaveLoader.SavingIsTemporarilyDisabled)
        {
            return false;
        }

        return SaveNameTracker.CanResolveSaveName();
    }

    // Live key state rather than Event.current.alt, which is not dependable on repaint events.
    private static bool QuitWithoutSavingHeld()
    {
        return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.AltGr);
    }

    // The labels are matched in the active language, the same way vanilla built them.
    private static void RefreshLabels()
    {
        if (labelsCachedFor == LanguageDatabase.activeLanguage)
        {
            return;
        }

        labelsCachedFor = LanguageDatabase.activeLanguage;
        quitToMainMenu = "QuitToMainMenu".Translate();
        quitToOS = "QuitToOS".Translate();
        saveAndQuitToMainMenu = "SaveAndQuitToMainMenu".Translate();
        saveAndQuitToOS = "SaveAndQuitToOS".Translate();
    }

    private static void SaveAndQuitToMainMenu()
    {
        Game game = Current.Game;
        string fileName = SaveNameTracker.SaveFileName();
        LongEventHandler.QueueLongEvent(delegate
        {
            if (SaveService.TrySave(game, fileName))
            {
                GenScene.GoToMainMenu();
            }
        }, "SavingLongEvent", false, null);
    }

    private static void SaveAndQuitToOS()
    {
        Game game = Current.Game;
        string fileName = SaveNameTracker.SaveFileName();
        LongEventHandler.QueueLongEvent(delegate
        {
            if (SaveService.TrySave(game, fileName))
            {
                LongEventHandler.ExecuteWhenFinished(Root.Shutdown);
            }
        }, "SavingLongEvent", false, null);
    }
}
