using System;
using System.IO;
using RimWorld;
using Verse;

namespace SaveOptions;

public static class SaveService
{
    private static string? expectedPath;
    private static bool succeeded;

    public static bool TrySave(Game game, string fileName)
    {
        if (Current.Game != game || Current.ProgramState != ProgramState.Playing ||
            GameDataSaveLoader.SavingIsTemporarilyDisabled)
        {
            return false;
        }

        expectedPath = Path.GetFullPath(GenFilePaths.FilePathForSavedGame(fileName));
        succeeded = false;
        try
        {
            GameDataSaveLoader.SaveGame(fileName);
            if (!succeeded)
            {
                Messages.Message("SaveOptions_SaveFailed".Translate(fileName), MessageTypeDefOf.RejectInput, false);
            }
            return succeeded;
        }
        catch (Exception exception)
        {
            Log.Error($"[Save Options] Failed to save '{fileName}': {exception}");
            return false;
        }
        finally
        {
            expectedPath = null;
        }
    }

    internal static void RecordSuccessfulSave(string path)
    {
        if (expectedPath != null && string.Equals(expectedPath, Path.GetFullPath(path), StringComparison.OrdinalIgnoreCase))
        {
            succeeded = true;
        }
    }
}
