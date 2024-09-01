using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ClownMeister.UnityEssentials.Logger.Enums;
using ClownMeister.UnityEssentials.Logger.Scriptables;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ClownMeister.UnityEssentials.Logger
{
    public static class DLogger
    {
        private static readonly LoggerSettings LoggerSettings = Resources.Load<LoggerSettings>("Settings/LoggerSettings");
        private static readonly ConcurrentQueue<Action> MainThreadQueue = new();
        private static readonly StringBuilder LOGBuffer = new();
        private static DateTime _lastFlushTime = DateTime.Now;

        public static void Warn(string message, LogChannel channel = LogChannel.General)
        {
            Log(message, channel, LogSeverity.Warn);
        }

        public static void Error(string message, LogChannel channel = LogChannel.General)
        {
            Log(message, channel, LogSeverity.Error);
        }

        public static void Log(string message, LogChannel channel = LogChannel.General, LogSeverity severity = LogSeverity.Info)
        {
            //TODO: add context ie planning tasks etc
            // If the log doesn't match active settings, simply return without further processing
            if (!LoggerSettings.activeSeverities.HasFlag(severity) || !LoggerSettings.activeChannels.HasFlag(channel)) return;

            Task.Run(() =>
            {
                // Any processing here is done on a background thread.
                // Add the log entry to the buffer
                string timestamp = DateTime.Now.ToString(LoggerSettings.timestampFormat);
                string formattedMessage = $"[{channel}] {message}";
                string fileMessage = $"{timestamp} {formattedMessage}";

                lock (LOGBuffer)
                {
                    LOGBuffer.AppendLine(fileMessage);
                }

                MainThreadQueue.Enqueue(() =>
                {
                    if (LoggerSettings.logToConsole)
                        // Log to console
                        switch (severity)
                        {
                            case LogSeverity.Debug:
                                Debug.Log(formattedMessage);
                                break;
                            case LogSeverity.Info:
                                Debug.Log(formattedMessage);
                                break;
                            case LogSeverity.Warn:
                                Debug.LogWarning(formattedMessage);
                                break;
                            case LogSeverity.Error:
                                Debug.LogError(formattedMessage);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
                        }

                    if (!LoggerSettings.logToFile) return;
                    // Decide whether to flush based on the log severity or time since last flush
                    if (severity <= LogSeverity.Info && !((DateTime.Now - _lastFlushTime).TotalSeconds >= 10)) return;
                    FlushLogBufferToFile();
                    _lastFlushTime = DateTime.Now;
                });
            });
        }
        private static void FlushLogBufferToFile()
        {
            string logFilePath = Path.Combine(Application.persistentDataPath, $"{LoggerSettings.logFileName}.log");
            string contentToWrite;

            lock (LOGBuffer)
            {
                contentToWrite = LOGBuffer.ToString();
                LOGBuffer.Clear();
            }

            // Write the current buffer content to the log file
            try
            {
                File.AppendAllText(logFilePath, contentToWrite);

                // Check if the file size exceeds the maximum allowed size after appending
                FileInfo fi = new(logFilePath);
                if (fi.Length <= LoggerSettings.maxLogSize * 1024 * 1024) return;
                string backupName = $"{LoggerSettings.logFileName}_backup_{DateTime.Now.ToString("yyyyMMddHHmmss")}.log";
                string backupPath = Path.Combine(Application.persistentDataPath, backupName);
                File.Move(logFilePath, backupPath);
            }
            catch (Exception exception)
            {
                // Handle the exception
                Debug.LogError($"Logging Error: {exception.Message}");
            }
        }

        // This method should be called once per frame
        public static void ProcessMainThreadQueue()
        {
            while (MainThreadQueue.TryDequeue(out Action action)) action.Invoke();
        }
    }
}