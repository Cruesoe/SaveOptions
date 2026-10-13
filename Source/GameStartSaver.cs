using RimWorld;
using Verse;

namespace SaveOptions;
public class GameStartSaver : GameComponent
{
    public const string StartSaveName = "Start";

    // Settle time after the last pod clears, and a cap so a scenario that never
    // uses pods (or leaves one parked) still gets its save.
    private const int SettleTicks = 60;
    private const int MaxWaitTicks = 2500;

    private bool savePending;
    private int ticksWaited;
    private int ticksSettled;

    public GameStartSaver(Game game)
    {
    }

    public override void StartedNewGame()
    {
        if (!SaveOptionsMod.Settings.SaveOnGameStart)
        {
            return;
        }

        savePending = true;
        ticksWaited = 0;
        ticksSettled = 0;
    }

    public override void GameComponentTick()
    {
        if (!SaveOptionsMod.Settings.SaveOnGameStart)
        {
            savePending = false;
        }

        if (!savePending)
        {
            return;
        }

        ticksWaited++;
        ticksSettled = ArrivalFinished() ? ticksSettled + 1 : 0;

        if (ticksSettled < SettleTicks && ticksWaited < MaxWaitTicks)
        {
            return;
        }

        if (GameDataSaveLoader.SavingIsTemporarilyDisabled)
        {
            return;
        }

        savePending = false;
        Game game = Current.Game;
        string fileName = GenFile.SanitizedFileName(StartSaveName);

        // Deferred rather than saved inline, the way vanilla's autosave does it.
        LongEventHandler.QueueLongEvent(delegate
        {
            if (SaveService.TrySave(game, fileName))
            {
                Messages.Message("SaveOptions_SavedStartFile".Translate(fileName), MessageTypeDefOf.SilentInput, false);
            }
        }, "SavingLongEvent", false, null);
    }

    private static bool ArrivalFinished()
    {
        foreach (Map map in Find.Maps)
        {
            if (map.listerThings.ThingsOfDef(ThingDefOf.DropPodIncoming).Count > 0)
            {
                return false;
            }

            if (map.listerThings.ThingsOfDef(ThingDefOf.ActiveDropPod).Count > 0)
            {
                return false;
            }
        }

        return true;
    }
}
