# Save Options implementation

Save Options supports RimWorld 1.6 and 1.7 from one source: `net472` against the 1.6 reference assemblies into `1.6/Assemblies`, and `net10.0` against the installed 1.7 game into `1.7/Assemblies`. Save and Quit and Save on Negative Event were adapted from Cruesoe's MIT-licensed repositories. Automatic loading is an independent implementation of the installed Auto Load Last Save mod's core behavior; no third-party source or assembly is shipped.

## Verified game behavior

Verified against the installed RimWorld 1.7 Assembly-CSharp.dll with ilspycmd:

- LetterStack.ReceiveLetter(Letter, string, int, bool) queues delayed letters and calls the same overload on arrival. The patch ignores delayed or hidden letters until received.
- GameDataSaveLoader.SaveGame catches exceptions from SafeSaver.Save. A postfix on SafeSaver.Save records successful completion so failed saves never trigger quitting or success messages.
- GameDataSaveLoader.CheckVersionAndLoadGame uses the game's normal version and mod checks before loading. Automatic loading uses this entry point.
- A load that throws is handled by GameAndMapInitExceptionHandlers.ErrorWhileLoadingGame, which returns to the main menu in the same process. The pending-load flag is cleared only when a game loads or starts, so a failed automatic load is not retried on the next launch.
- Root_Entry.Start performs the developer-mode autostart check once; automatic loading waits for the main menu instead of interfering with that check.
- GenScene.GoToMainMenu clears queued events and transitions through the game's normal cleanup. Automatic loading runs at most once per process.
- ScribeExtractor resolves every saved class name through BackCompatibility.GetBackCompatibleType and logs "Could not find class" for unknown ones. A postfix maps Save and Quit's SaveNameTracker and GameStartSaver to their Save Options equivalents.
- Only Assembly-CSharp is patchable in 1.7. The optional Letter Permanent Injury exclusion recognizes its call stack instead of patching that mod's assembly.

The 1.6 reference assemblies have no method bodies, so the behavior above was read from 1.7 code. For 1.6, the signatures and parameter names of every patched and called method were checked against Krafs.Rimworld.Ref 1.6.4871 and match.

Cooldown (1-200 seconds), event-label length (80 characters), and starting-colony wait thresholds retain the existing save mods' settings and behavior. Countdown duration (1-60 seconds) follows the installed Auto Load Last Save options. These are mod design bounds, not claimed engine limits.

## Defaults

Negative-event saving, save-and-quit, and automatic loading are enabled. The startup countdown is three seconds. Event suffixes, starting-colony saves, and verbose loading logs are disabled. Settings are independent of the original mods; existing colonies need no conversion.
