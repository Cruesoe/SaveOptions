using System;
using System.IO;

namespace Verse
{
    public sealed class Game { }
    public enum ProgramState { Entry, Playing }
    public static class Current
    {
        public static Game? Game;
        public static ProgramState ProgramState;
    }
    public static class GameDataSaveLoader
    {
        public static bool SavingIsTemporarilyDisabled;
        public static Action<string> Save = _ => { };
        public static void SaveGame(string fileName) => Save(fileName);
    }
    public static class GenFilePaths
    {
        public static string FilePathForSavedGame(string name) => Path.Combine(Path.GetTempPath(), name + ".rws");
    }
    public static class Log
    {
        public static int Errors;
        public static void Error(string message) => Errors++;
    }
    public static class Messages
    {
        public static int Count;
        public static void Message(string text, object type, bool historical) => Count++;
    }
    public static class Translator
    {
        public static string Translate(this string key, string argument) => key + argument;
    }
}

namespace RimWorld
{
    public static class MessageTypeDefOf
    {
        public static readonly object RejectInput = new();
    }
}
