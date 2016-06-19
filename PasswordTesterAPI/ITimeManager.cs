using System;

namespace PasswordTesterAPI
{
    /// <summary>
    /// Interface to define the access to the current epoch time of the system. Intended to be used by PasswordManager
    /// </summary>
    public interface ITimeManager
    {
        /// <summary>
        /// Gets the current time in epoch (milliseconds elapsed since 1/1/1970)
        /// </summary>
        long CurrentEpochMS { get; }
    }
}
