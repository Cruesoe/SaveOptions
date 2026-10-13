# Save Options implementation

Save Options targets RimWorld 1.7 only. Save and Quit and Save on Negative Event were adapted from Cruesoe's MIT-licensed repositories. Automatic loading is an independent implementation of the installed Auto Load Last Save mod's core behavior; no third-party source or assembly is shipped.

## Verified game behavior

Verified against installed RimWorld 1.7.4976 rev1043 Assembly-CSharp.dll with ilspycmd on October 13, 2026:

- LetterStack.ReceiveLetter(Letter, string, int, bool) queues delayed letters and calls the same overload on arrival. The patch ignores delayed or hidden letters until received.
- GameDataSaveLoader.SaveGame catches exceptions from SafeSaver.Save. A postfix on SafeSaver.Save records successful completion so failed saves never trigger quitting or success messages.
- GameDataSaveLoader.CheckVersionAndLoadGame uses the game's normal version and mod checks before loading. Automatic loading uses this entry point.
- Root_Entry.Start performs the developer-mode autostart check once; automatic loading waits for the main menu instead of interfering with that check.
- GenScene.GoToMainMenu clears queued events and transitions through the game's normal cleanup. Automatic loading runs at most once per process.
- Only Assembly-CSharp is patchable in 1.7. The optional Letter Permanent Injury exclusion recognizes its call stack instead of patching that mod's assembly.

Cooldown (1-200 seconds), event-label length (80 characters), and starting-colony wait thresholds retain the existing save mods' settings and behavior. Countdown duration (1-60 seconds) follows the installed Auto Load Last Save options. These are mod design bounds, not claimed engine limits.

## Defaults

Negative-event saving, save-and-quit, and automatic loading are enabled. The startup countdown is three seconds. Event suffixes, starting-colony saves, and verbose loading logs are disabled. Settings are independent of the original mods; existing colonies need no conversion.

## Validation

Tools/Test-All.ps1 checks event filename safety, empty save directories, selection of newer autosaves and event saves, stable ties, and translation references. Build and deployment validate all shipped XML and compare deployed files by SHA-256.

Initial validation on October 13, 2026: Release build passed with zero warnings and zero errors; 25 assertions passed, including successful, swallowed-failure, thrown-failure, wrong-game, wrong-scene, blocked-save, and wrong-file save outcomes. The test substitutes the game entry points and runs the actual SaveService code; it does not replace an in-game save test. Deployment verified all five runtime files with no missing, extra, or mismatched files. An isolated 1.7 launch with Harmony, Core, the installed expansions, and Save Options recorded no errors, ReflectionTypeLoadException, HarmonyException, or SaveOptions stack frames. The isolated process was closed afterward. Logs and temporary save data remain under ignored TestResults.

In-game checks still required: event saves while paused, cooldown and blocked-saving behavior, both quit targets, Alt bypass, commitment mode, start saves with and without drop pods, startup cancellation, failed-load recovery, and load warnings when the save's mod list differs.
