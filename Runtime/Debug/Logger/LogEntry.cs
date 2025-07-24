using System;

namespace UniUtils.Debugging
{
    /// <summary>
    /// Represents a log entry.
    /// </summary>
    [Serializable]
    public struct LogEntry
    {
        public string type;
        public DateTime dateTime;
        public string logString;
        public string stackTrace;

        public LogEntry(string logType, DateTime time, string log, string stack)
        {
            type = logType;
            dateTime = time;
            logString = log;
            stackTrace = stack;
        }
    }
}