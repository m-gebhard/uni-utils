using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UniUtils.Data;
using UniUtils.Extensions;
using UniUtils.GameObjects;

namespace UniUtils.Debugging
{
    /// <summary>
    /// A logger class that persists log entries and saves them to a file.
    /// </summary>
    /// <example>
    /// <code>
    /// Access the logger instance (singleton) and retrieve the current log as a string.
    /// string currentLog = Logger.Instance.Log;
    ///
    /// You can also customize ignored words and toggle stack trace appending in the inspector.
    /// </code>
    /// </example>
    [DefaultExecutionOrder(-1000)]
    public class Logger : PersistentSingleton<Logger>
    {
        #region Fields

        [Header("Log File")]
        [Tooltip("The prefix for the log file name.")]
        [SerializeField] private string logFilePrefix = "log_";
        [Tooltip("The folder where log files will be saved.")]
        [SerializeField] private string logFileFolder = "logs";
        [Header("Build")]
        [Tooltip("If enabled, the logger will be active in builds. Otherwise, it will only log in the editor.")]
        [SerializeField] private bool enableInBuilds;
        [Header("Content & Filtering")]
        [Tooltip("The types of log messages to include in the log file.")]
        [SerializeField] private ELogTypeMask logTypeFilter = ELogTypeMask.All;
        [Tooltip("Log entries containing these words will be ignored.")]
        [SerializeField] private List<string> ignoredWords = new();
        [Tooltip("If enabled, stack traces will be appended to log entries.")]
        [SerializeField] private bool appendStackToLogs;
        [Tooltip("If enabled, the logger will include system information in the log file header.")]
        [SerializeField] private bool includeSystemInfo = true;
        [Header("Saving")]
        [Tooltip(
            "If enabled, the log file will only be written when the application exits (most efficient but crashes could lead to loss). Otherwise it will update with every new log entry.")]
        [SerializeField] private bool onlyWriteOnExit;
        [Tooltip(
            "If enabled, the log file will be automatically flushed (costs performance) (onlyWriteOnExit has to be disabled).")]
        [SerializeField] private bool enableAutoFlush;
        [Tooltip(
            "The interval in seconds for manual flushing the log file (if enableAutoFlush and onlyWriteOnExit are disabled).")]
        [SerializeField] private float manualFlushInterval = 30f;

        private readonly List<LogEntry> entries = new();

        private DateTime startTime;
        private string logName = "";

        private FileHandle logFileHandle;
        private StreamWriter logWriter;

        /// <summary>
        /// Gets the complete log as a string.
        /// </summary>
        public string LogString => GetLogFileHead() + "\n" + CreateReadableLog();

        public event Action<LogEntry> OnNewLogEntry;

        #endregion

        /// <summary>
        /// Initializes the logger.
        /// </summary>
        protected override void Awake()
        {
#if !UNITY_EDITOR
            if (!enableInBuilds)
            {
                Destroy(this);
                return;
            }
#endif
            base.Awake();

            startTime = DateTime.Now;
            logName = $"{logFilePrefix}{startTime:yyyyMMdd\\THHmmss}";
            logFileHandle = new FileHandle($"{logFileFolder}/{logName}.log");

            if (!onlyWriteOnExit)
            {
                try
                {
                    FileStream stream = logFileHandle.OpenWriteStream(overwrite: true);
                    logWriter = new StreamWriter(stream) { AutoFlush = enableAutoFlush };
                    logWriter.WriteLine(GetLogFileHead());
                }
                catch (Exception e)
                {
                    this.LogError($"Failed to open log file stream: {e}");
                }

                if (!enableAutoFlush)
                {
                    InvokeRepeating(nameof(FlushLogFile), manualFlushInterval, manualFlushInterval);
                }
            }

            Application.logMessageReceived += OnLogEntry;
        }

        /// <summary>
        /// Cleans up the logger.
        /// </summary>
        private void OnDestroy()
        {
            Application.logMessageReceived -= OnLogEntry;
            CancelInvoke();

            if (logWriter != null)
            {
                FlushLogFile();
                logWriter.Close();
                logWriter.Dispose();
                logWriter = null;
            }
        }

        /// <summary>
        /// Handles the application quitting event to ensure the log file is written.
        /// </summary>
        private void OnApplicationQuit()
        {
            if (logWriter == null && onlyWriteOnExit)
            {
                logFileHandle.WriteText(LogString);
            }
        }

        /// <summary>
        /// Flushes the log file to ensure all entries are written.
        /// </summary>
        private void FlushLogFile()
        {
            logWriter?.Flush();
        }

        /// <summary>
        /// Gets the header for the log file.
        /// </summary>
        /// <returns>A string containing the log file header.</returns>
        private string GetLogFileHead()
        {
            string sysInfo = includeSystemInfo ? GetSystemInfoString() : "";
            return $"Application Version: {Application.version} - Time: {startTime}\n{sysInfo}";
        }

        /// <summary>
        /// Retrieves detailed system information as a formatted string.
        /// </summary>
        /// <returns>
        /// A string containing information about the device, operating system, CPU, GPU, and memory.
        /// </returns>
        private static string GetSystemInfoString()
        {
            return $"Device: {SystemInfo.deviceModel} ({SystemInfo.deviceType})\n" +
                   $"OS: {SystemInfo.operatingSystem} ({SystemInfo.operatingSystemFamily})\n" +
                   $"CPU: {SystemInfo.processorType} ({SystemInfo.processorCount} Cores - {SystemInfo.processorFrequency} MHz)\n" +
                   $"GPU: {SystemInfo.graphicsDeviceName} (Memory: {SystemInfo.graphicsMemorySize} MB), " +
                   $"Version: {SystemInfo.graphicsDeviceVersion}, API: {SystemInfo.graphicsShaderLevel}\n" +
                   $"Memory: {SystemInfo.systemMemorySize} MB\n";
        }

        /// <summary>
        /// Handles a new log entry.
        /// </summary>
        /// <param name="logString">The log message.</param>
        /// <param name="stackTrace">The stack trace.</param>
        /// <param name="type">The type of log message.</param>
        private void OnLogEntry(string logString, string stackTrace, LogType type)
        {
            if (string.IsNullOrEmpty(logString)) return;
            if (!logTypeFilter.HasFlag(ToLogTypeMask(type))) return;
            if (ContainsIgnoredWord(logString) || ContainsIgnoredWord(stackTrace)) return;

            string stack = appendStackToLogs ? stackTrace : "";

            DateTime time = DateTime.Now;
            LogEntry entry = new(type.ToString(), time, logString, stack);
            entries.Add(entry);

            if (logWriter != null)
            {
                logWriter.WriteLine($"[{time}] {type}: {logString}");
                if (!string.IsNullOrWhiteSpace(stack))
                {
                    logWriter.WriteLine(stack);
                }
            }

            OnNewLogEntry?.Invoke(entry);
        }

        /// <summary>
        /// Creates a readable log string from the log entries.
        /// </summary>
        /// <returns>A string containing the readable log.</returns>
        private string CreateReadableLog()
        {
            StringBuilder sb = new();

            foreach (LogEntry entry in entries)
            {
                sb.AppendLine($"[{entry.dateTime}] {entry.type}: {entry.logString}");

                if (!string.IsNullOrWhiteSpace(entry.stackTrace))
                    sb.AppendLine(entry.stackTrace);
            }

            return sb.ToString();
        }


        /// <summary>
        /// Checks if the given text contains any ignored words.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>True if the text contains any ignored words, otherwise false.</returns>
        private bool ContainsIgnoredWord(string text)
        {
            foreach (string word in ignoredWords)
            {
                if (text.Contains(word))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Converts a Unity <see cref="LogType"/> to a corresponding <see cref="ELogTypeMask"/> value.
        /// </summary>
        /// <param name="type">The <see cref="LogType"/> to convert.</param>
        /// <returns>The corresponding <see cref="ELogTypeMask"/> value.</returns>
        private static ELogTypeMask ToLogTypeMask(LogType type)
        {
            return type switch
            {
                LogType.Log => ELogTypeMask.Log,
                LogType.Warning => ELogTypeMask.Warning,
                LogType.Error => ELogTypeMask.Error,
                LogType.Assert => ELogTypeMask.Assert,
                LogType.Exception => ELogTypeMask.Exception,
                _ => ELogTypeMask.None,
            };
        }
    }
}