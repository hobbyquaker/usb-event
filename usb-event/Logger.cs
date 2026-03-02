using System;
using System.IO;

// ── Logger ────────────────────────────────────────────────────────────────────

sealed class Logger : IDisposable
{
    readonly object        _lock = new();
    readonly bool          _toConsole;
    readonly StreamWriter? _file;

    public Logger(bool toConsole, string? logFile = null)
    {
        _toConsole = toConsole;
        if (logFile is not null)
            _file = new StreamWriter(logFile, append: true, System.Text.Encoding.UTF8) { AutoFlush = true };
    }

    public void Write(string message, ConsoleColor color = ConsoleColor.Gray)
    {
        lock (_lock)
        {
            _file?.WriteLine(message);
            if (!_toConsole) return;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }

    public void Dispose() => _file?.Dispose();
}
