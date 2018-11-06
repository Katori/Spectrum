// A simple logger class that uses Console.WriteLine by default.
// Can also do Logger.LogMethod = Debug.Log for Unity etc.
// (this way we don't have to depend on UnityEngine.DLL and don't need a
//  different version for every UnityEngine version here)
using System;
using System.Diagnostics;

namespace Telepathy
{
    public static class Logger
    {
        // log regular
        public static Action<string> LogMethod = Console.WriteLine;
        public static void Log(string msg)
        {
			Debug.WriteLine(msg);
        }

        // log warning
        public static Action<string> LogWarningMethod = Console.WriteLine;
        public static void LogWarning(string msg)
        {
			Debug.WriteLine(msg);
		}

        // log error
        public static Action<string> LogErrorMethod = Console.Error.WriteLine;
        public static void LogError(string msg)
        {
			Debug.WriteLine(msg);
		}
    }
}