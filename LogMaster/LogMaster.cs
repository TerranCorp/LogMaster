using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LogMaster
{
    public class LogMaster
    {
        private bool _isTimeStampEnabled;
        private bool _isLogLevelEnabled;
        private bool _isConsoleLogEnabled;
        private bool _isFileLogEnabled;
        private bool _isLineNumberEnabled;
        private bool _isMethodNameEnabled;
        private bool _isFileNameEnabled;

        private string _fileLogPath;


        private LogLevel logLevel;

        private readonly object _lock = new object();

        public LogMaster() { }

        #region Get/Set
        public bool IsTimeStampEnabled1 { get => _isTimeStampEnabled; set => _isTimeStampEnabled = value; }
        public bool IsLogLevelEnabled1 { get => _isLogLevelEnabled; set => _isLogLevelEnabled = value; }
        public bool IsConsoleLogEnabled { get => _isConsoleLogEnabled; set => _isConsoleLogEnabled = value; }
        public bool IsFileLogEnabled { get => _isFileLogEnabled; set => _isFileLogEnabled = value; }
        public string FileLogPath { get => _fileLogPath; set => _fileLogPath = value; }
        public bool IsLineNumberEnabled { get => _isLineNumberEnabled; set => _isLineNumberEnabled = value; }
        public bool IsMethodNameEnabled { get => _isMethodNameEnabled; set => _isMethodNameEnabled = value; }
        public bool IsFileNameEnabled { get => _isFileNameEnabled; set => _isFileNameEnabled = value; }
        #endregion

        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void RainbowLog(string message, LogLevel log = LogLevel.Info, int rainbowSteps = 50)
        {
            if (!_isConsoleLogEnabled) return;

            // Enable timestamp + log level if needed
            string formattedMessage = message;
            if (_isTimeStampEnabled && _isLogLevelEnabled)
                formattedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{log}] - {message}";
            else if (_isTimeStampEnabled)
                formattedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] - {message}";
            else if (_isLogLevelEnabled)
                formattedMessage = $"[{log}] - {message}";

            // Append caller info if flags are enabled
            formattedMessage = AppendCallerInfo(formattedMessage, _isLineNumberEnabled, _isMethodNameEnabled, _isFileNameEnabled);

            // Generate rainbow colors
            List<(int r, int g, int b)> colors = RainbowGenerator.GenerateRainbow(rainbowSteps);

            lock (_lock)
            {
                int colorIndex = 0;
                int colorCount = colors.Count;

                foreach (char c in formattedMessage)
                {
                    var (r, g, b) = colors[colorIndex % colorCount];
                    Console.Write($"\u001b[38;2;{r};{g};{b}m{c}");
                    colorIndex++;
                }

                Console.WriteLine("\u001b[0m"); // Reset color
            }

            // Also log to file if enabled
            if (_isFileLogEnabled && !string.IsNullOrWhiteSpace(_fileLogPath))
            {
                try
                {
                    File.AppendAllText(_fileLogPath, formattedMessage + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[Logger Error] Failed to write to file: {ex.Message}");
                }
            }
        }

        public void RainbowAnimateLogSmooth(string message, LogLevel log = LogLevel.Info, int rainbowSteps = 50, int frameDelayMs = 50, bool waitForKey = false)
        {
            if (!_isConsoleLogEnabled) return;

            string formattedMessage = message;
            if (_isTimeStampEnabled && _isLogLevelEnabled)
                formattedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{log}] - {message}";
            else if (_isTimeStampEnabled)
                formattedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] - {message}";
            else if (_isLogLevelEnabled)
                formattedMessage = $"[{log}] - {message}";

            formattedMessage = AppendCallerInfo(formattedMessage, _isLineNumberEnabled, _isMethodNameEnabled, _isFileNameEnabled);

            int length = formattedMessage.Length;
            bool keyPressed = false;

            Console.CursorVisible = false;
            var originalLeft = Console.CursorLeft;
            var originalTop = Console.CursorTop;

            Console.SetCursorPosition(originalLeft, originalTop);

            while (!keyPressed)
            {
                lock (_lock)
                {
                    for (int i = 0; i < length; i++)
                    {
                        double hue = ((i * 360.0 / length) + DateTime.Now.Millisecond / 1000.0 * 360) % 360;
                        var (r, g, b) = RainbowGenerator.HsvToRgb(hue, 1.0, 1.0);
                        Console.Write($"\u001b[38;2;{r};{g};{b}m{formattedMessage[i]}");
                    }

                    Console.Write("\u001b[0m"); // Reset color
                    Console.SetCursorPosition(originalLeft, originalTop); // stay on the same line
                }

                System.Threading.Thread.Sleep(frameDelayMs);

                // Check if a key has been pressed (non-blocking)
                if (waitForKey && Console.KeyAvailable)
                {
                    Console.ReadKey(true); // consume the key
                    keyPressed = true;
                }
            }

            Console.SetCursorPosition(originalLeft, originalTop + 1); // move to next line
            Console.CursorVisible = true;

            // Optional: log plain message to file
            if (_isFileLogEnabled && !string.IsNullOrWhiteSpace(_fileLogPath))
            {
                try
                {
                    File.AppendAllText(_fileLogPath, formattedMessage + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[Logger Error] Failed to write to file: {ex.Message}");
                }
            }
        }
        public void RainbowAnimateLogFrame(string message, LogLevel log = LogLevel.Info, int rainbowSteps = 50)
        {
            if (!_isConsoleLogEnabled) return;

            string formattedMessage = message;
            if (_isTimeStampEnabled && _isLogLevelEnabled)
                formattedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{log}] - {message}";
            else if (_isTimeStampEnabled)
                formattedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] - {message}";
            else if (_isLogLevelEnabled)
                formattedMessage = $"[{log}] - {message}";

            formattedMessage = AppendCallerInfo(formattedMessage, _isLineNumberEnabled, _isMethodNameEnabled, _isFileNameEnabled);

            int length = formattedMessage.Length;

            lock (_lock)
            {
                var originalLeft = Console.CursorLeft;
                var originalTop = Console.CursorTop;
                Console.SetCursorPosition(originalLeft, originalTop);

                for (int i = 0; i < length; i++)
                {
                    double hue = ((i * 360.0 / length) + DateTime.Now.Millisecond / 1000.0 * 360) % 360;
                    var (r, g, b) = RainbowGenerator.HsvToRgb(hue, 1.0, 1.0);
                    Console.Write($"\u001b[38;2;{r};{g};{b}m{formattedMessage[i]}");
                }

                Console.Write("\u001b[0m"); // reset color
                originalTop = (originalTop + 1) % Console.BufferHeight; // wrap to top
                Console.SetCursorPosition(originalLeft, originalTop);

            }
        }

        public void Log(string message, (int r, int g, int b)? color = null, LogLevel logLevel = LogLevel.Info)
        {
            if (_isTimeStampEnabled)
            {
                if (!_isLogLevelEnabled)
                {
                    message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] - {message}";
                }

                message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{logLevel}] - {message}";
            }

            message = AppendCallerInfo(message, _isLineNumberEnabled, _isMethodNameEnabled, _isFileNameEnabled);

            lock (_lock)
            {
                if (_isConsoleLogEnabled)
                {
                    if (color.HasValue)
                    {
                        var (r, g, b) = color.Value;
                        Console.Write($"\u001b[38;2;{r};{g};{b}m"); // set RGB text color
                        Console.WriteLine(message);
                        Console.Write("\u001b[0m"); // reset
                    }
                    else
                    {
                        // fallback: choose a default
                        var (r, g, b) = GetRgbForLevel(logLevel);
                        Console.Write($"\u001b[38;2;{r};{g};{b}m");
                        Console.WriteLine(message);
                        Console.Write("\u001b[0m");
                    }
                }

                if (_isFileLogEnabled)
                {
                    File.AppendAllText(_fileLogPath, message + Environment.NewLine);
                }
            }
        }

        private (int r, int g, int b) GetRgbForLevel(LogLevel level)
        {
            return level switch
            {
                LogLevel.Info => (0, 200, 255),      // cyan
                LogLevel.Warning => (255, 165, 0),   // orange
                LogLevel.Error => (255, 0, 0),       // red
                LogLevel.Debug => (160, 160, 160),   // gray
                LogLevel.Custom => (0, 255, 0),      // green
                _ => (255, 255, 255)
            };
        }

        private string AppendCallerInfo(
        string message,
        bool includeLineNumber = false,
        bool includeMethodName = false,
        bool includeFileName = false,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string methodName = "",
        [CallerFilePath] string filePath = "")
        {
            var parts = new List<string> { message };

            if (includeLineNumber)
                parts.Add($"Line: {lineNumber}");
            if (includeMethodName)
                parts.Add($"Method: {methodName}");
            if (includeFileName)
                parts.Add($"File: {System.IO.Path.GetFileName(filePath)}");

            return string.Join(" | ", parts);
        }

        public void PrintRainbowAsciiHeader()
        {
            if (!_isConsoleLogEnabled) return;

            string[] asciiArt = new string[]
            {
                @"░▒▓█▓▒░      ░▒▓██████▓▒░ ░▒▓██████▓▒░░▒▓██████████████▓▒░ ░▒▓██████▓▒░ ░▒▓███████▓▒░▒▓████████▓▒░▒▓████████▓▒░▒▓███████▓▒░  ",
                @"░▒▓█▓▒░     ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░         ░▒▓█▓▒░   ░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░ ",
                @"░▒▓█▓▒░     ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░         ░▒▓█▓▒░   ░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░ ",
                @"░▒▓█▓▒░     ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒▒▓███▓▒░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓████████▓▒░░▒▓██████▓▒░   ░▒▓█▓▒░   ░▒▓██████▓▒░ ░▒▓███████▓▒░  ",
                @"░▒▓█▓▒░     ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░  ░▒▓█▓▒░   ░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░ ",
                @"░▒▓█▓▒░     ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░  ░▒▓█▓▒░   ░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░ ",
                @"░▒▓████████▓▒░▒▓██████▓▒░ ░▒▓██████▓▒░░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓███████▓▒░   ░▒▓█▓▒░   ░▒▓████████▓▒░▒▓█▓▒░░▒▓█▓▒░ ",
            };

            lock (_lock)
            {
                Console.CursorVisible = false;
                Random random = new();
                int pos = random.Next(15);
                int startRow = pos; // always top of console

                for (int row = 0; row < asciiArt.Length; row++)
                {
                    Console.SetCursorPosition(pos, startRow + row);
                    string line = asciiArt[row];

                    for (int i = 0; i < line.Length; i++)
                    {
                        double hue = (i * 360.0 / line.Length + DateTime.Now.Millisecond / 1000.0 * 360) % 360;
                        var (r, g, b) = RainbowGenerator.HsvToRgb(hue, 1.0, 1.0);
                        Console.Write($"\u001b[38;2;{r};{g};{b}m{line[i]}");
                    }

                    Console.Write("\u001b[0m"); // reset color
                }

                // leave cursor below header for logging
                Console.SetCursorPosition(0, startRow + asciiArt.Length);
                Console.CursorVisible = true;
            }
        }

    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Debug,
        Custom
    }
}