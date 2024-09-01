using System;

namespace ClownMeister.UnityEssentials.Logger.Enums
{
    [Flags]
    public enum LogChannel
    {
        General = 1 << 0,
        Navigation = 1 << 1,
        Tasks = 1 << 2,
        Building = 1 << 3,
        Items = 1 << 4,
    }
}