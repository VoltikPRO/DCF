using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DCF.Helpers
{
    public static class DebugHelper
    {
        public static void Log(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
#if DEBUG
            string fileName = System.IO.Path.GetFileName(filePath);

            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] " +
                            $"{fileName}:{lineNumber} ({memberName}) → {message}");
#endif
        }
    }
}
