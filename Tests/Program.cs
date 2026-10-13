using System;
using System.IO;
using SaveOptions;
using Verse;

int assertions = 0;
void Equal<T>(T expected, T actual)
{
    assertions++;
    if (!Equals(expected, actual))
    {
        throw new Exception($"Expected '{expected}', got '{actual}'.");
    }
}

Equal("Bad Event", SavePolicies.EventSaveName("Raid", false));
Equal("Bad Event_Raid", SavePolicies.EventSaveName("Raid", true));
Equal("Bad Event_Raid_Pirates", SavePolicies.EventSaveName("Raid:/\\?*Pirates", true));
Equal("Bad Event", SavePolicies.EventSaveName("<>:\"/\\|?* .", true));
Equal("Bad Event_Injury_Scar", SavePolicies.EventSaveName("Injury\n\tScar", true));
Equal("Bad Event_Raid", SavePolicies.EventSaveName(" ..Raid... ", true));
Equal(90, SavePolicies.EventSaveName(new string('A', 200), true).Length);
Equal<FileInfo?>(null, SavePolicies.LatestSave(Array.Empty<FileInfo>()));

string directory = Path.Combine(Path.GetTempPath(), "SaveOptionsTests-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(directory);
try
{
    string manual = Path.Combine(directory, "Colony.rws");
    string autosave = Path.Combine(directory, "Autosave-1.rws");
    string negative = Path.Combine(directory, "Bad Event.rws");
    foreach (string path in new[] { manual, autosave, negative })
    {
        File.WriteAllText(path, "test");
    }
    DateTime timestamp = new(2026, 10, 13, 12, 0, 0, DateTimeKind.Utc);
    File.SetLastWriteTimeUtc(manual, timestamp);
    File.SetLastWriteTimeUtc(autosave, timestamp.AddMinutes(1));
    File.SetLastWriteTimeUtc(negative, timestamp.AddMinutes(-1));
    Equal(autosave, SavePolicies.LatestSave(new DirectoryInfo(directory).GetFiles("*.rws"))?.FullName);
    File.SetLastWriteTimeUtc(negative, timestamp.AddMinutes(2));
    Equal(negative, SavePolicies.LatestSave(new DirectoryInfo(directory).GetFiles("*.rws"))?.FullName);
    File.SetLastWriteTimeUtc(manual, timestamp.AddMinutes(2));
    Equal(negative, SavePolicies.LatestSave(new[] { new FileInfo(manual), new FileInfo(negative) })?.FullName);
    Equal(negative, SavePolicies.LatestSave(new[] { new FileInfo(negative), new FileInfo(manual) })?.FullName);
}
finally
{
    Directory.Delete(directory, true);
}

Game game = new();
Current.Game = game;
Current.ProgramState = ProgramState.Playing;
int saveCalls = 0;
GameDataSaveLoader.Save = name => saveCalls++;
Equal(false, SaveService.TrySave(new Game(), "Colony"));
Equal(0, saveCalls);
Current.ProgramState = ProgramState.Entry;
Equal(false, SaveService.TrySave(game, "Colony"));
Equal(0, saveCalls);
Current.ProgramState = ProgramState.Playing;
GameDataSaveLoader.SavingIsTemporarilyDisabled = true;
Equal(false, SaveService.TrySave(game, "Colony"));
Equal(0, saveCalls);
GameDataSaveLoader.SavingIsTemporarilyDisabled = false;
Equal(false, SaveService.TrySave(game, "Colony"));
Equal(1, Messages.Count);
GameDataSaveLoader.Save = name => SaveService.RecordSuccessfulSave(GenFilePaths.FilePathForSavedGame("Other"));
Equal(false, SaveService.TrySave(game, "Colony"));
GameDataSaveLoader.Save = name => SaveService.RecordSuccessfulSave(GenFilePaths.FilePathForSavedGame(name));
Equal(true, SaveService.TrySave(game, "Colony"));
GameDataSaveLoader.Save = name => { };
Equal(false, SaveService.TrySave(game, "Colony"));
GameDataSaveLoader.Save = name => throw new IOException("Test save failure");
Equal(false, SaveService.TrySave(game, "Colony"));
Equal(1, Log.Errors);

Console.WriteLine($"Passed {assertions} assertions.");
