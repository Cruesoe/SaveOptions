using RimWorld;
using Verse;

namespace SaveOptions;
public class SaveNameTracker : GameComponent
{
    private string? generatedName;

    public SaveNameTracker(Game game)
    {
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref generatedName, "generatedName");
    }
    public static bool CanResolveSaveName()
    {
        return Current.Game?.World != null && Find.FactionManager != null && Faction.OfPlayer != null;
    }
    public static string SaveFileName()
    {
        Faction player = Faction.OfPlayer;
        string name = player.HasName ? player.Name : GeneratedName(player);
        return GenFile.SanitizedFileName(name);
    }

    private static string GeneratedName(Faction player)
    {
        SaveNameTracker? tracker = Current.Game?.GetComponent<SaveNameTracker>();
        if (tracker == null)
        {
            return SaveGameFilesUtility.UnusedDefaultFileName(player.def.LabelCap);
        }

        if (tracker.generatedName.NullOrEmpty())
        {
            tracker.generatedName = SaveGameFilesUtility.UnusedDefaultFileName(player.def.LabelCap);
        }

        return tracker.generatedName!;
    }
}
