using Verse;

namespace SaveOptions;

public sealed class AutoLoadRecovery : GameComponent
{
    public AutoLoadRecovery(Game game) { }

    public override void LoadedGame() => AutoLoadLastSave.ClearPendingLoad();
    public override void StartedNewGame() => AutoLoadLastSave.ClearPendingLoad();
}
