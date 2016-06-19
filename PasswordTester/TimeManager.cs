using System;

namespace PasswordTesterAPI
{
    /// <summary>
    /// Implementation of ITimeManager that uses the real clock time from the computer
    /// </summary>
    public class TimeManager : ITimeManager
    {
        // From ITimeManager
        //@{
        public long CurrentEpochMS
        {
            get
            {
                TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                return (long)t.TotalMilliseconds;
            }
        }
        //@}
    }
}
