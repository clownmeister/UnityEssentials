using ClownMeister.UnityEssentials.Logger.Enums;
using UnityEngine;

namespace ClownMeister.UnityEssentials.Logger.Scriptables
{
    [CreateAssetMenu(fileName = "LoggerSettings", menuName = "ClownMeister/Settings/LoggerSettings", order = 100)]
    public class LoggerSettings : ScriptableObject
    {
        [EnumFlags] public LogChannel activeChannels = 0; // or LogChannel.All if you want all channels active by default

        [EnumFlags] public LogSeverity activeSeverities = 0; // Sets the default to these three.

        public string logFileName = "main"; // Default format, can be changed in the inspector
        public string timestampFormat = "yyyy-MM-dd HH:mm:ss"; // Default format, can be changed in the inspector
        [Header("Size in MB")]
        public float maxLogSize = 10;
        public bool logToConsole = true;
        public bool logToFile = true;
    }
}