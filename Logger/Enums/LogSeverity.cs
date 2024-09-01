using System;

namespace ClownMeister.UnityEssentials.Logger.Enums
{
    [Flags]
    public enum LogSeverity
    {
        Debug = 1 << 0, // Detailed information typically of interest only when diagnosing problems.
        Info = 1 << 1, // Confirmation that things are working as expected.
        Warn = 1 << 2, // An event that will likely lead to an error or issue.
        Error = 1 << 3, // A severe issue that impeded the normal flow. Indicates a major subsystem failure or system instability.
    }
}