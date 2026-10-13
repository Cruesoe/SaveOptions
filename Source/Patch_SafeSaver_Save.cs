using HarmonyLib;
using Verse;

namespace SaveOptions;

// SaveGame catches exceptions; this postfix runs only after the file was saved successfully.
[HarmonyPatch(typeof(SafeSaver), nameof(SafeSaver.Save))]
public static class Patch_SafeSaver_Save
{
    public static void Postfix(string path) => SaveService.RecordSuccessfulSave(path);
}
