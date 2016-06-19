using System;
using System.Collections.Generic;
using System.Linq;

namespace PasswordTesterAPI
{
    /// <summary>
    /// Password Manager implementation to create and check the validity of passwords
    /// </summary>
    public class PasswordManager : IPasswordManager
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="timeManager">TimeManager implementation that this instance uses</param>
        public PasswordManager(ITimeManager timeManager)
        {
            if (timeManager == null)
                throw new Exception("Must set a valid TimeManager implementation");

            _timeManager = timeManager;
        }

        // From IPassworManager
        //@{
        public string CreatePassword(uint id)
        {
            long currentEpoch = _timeManager.CurrentEpochMS;
            PasswordStorage passwordStorage = null;

            if (_passwords.ContainsKey(id))
            {
                passwordStorage = _passwords[id];
                if (IsTimeValid(passwordStorage.epochCreated, currentEpoch))
                {
                    // The password is still valid, so return it (so current clients can use it) and update time
                    // NOTE: A continous petition to CreatePassword for the same ID could lead to a password
                    // than never changes. If this is not desired (depending on requirements), I see two solutions:
                    // 1.- Change the password (but then the current one becomes invalid before the 30 seconds expires)
                    // 2.- Use a list as the value of _passwords so it can store multiple PasswordStorage for a given ID
                    passwordStorage.epochCreated = currentEpoch;
                    return passwordStorage.password;
                }
                else
                {
                    // The current password is invalid, so let the code continue to create a new one later
                }
            }
            else
            {
                passwordStorage = new PasswordStorage();
                _passwords[id] = passwordStorage;
            }

            passwordStorage.epochCreated = currentEpoch;
            passwordStorage.password =
                ((char)_random.Next(65, 90)).ToString() +
                ((char)_random.Next(65, 90)).ToString() +
                ((char)_random.Next(65, 90)).ToString();

            return passwordStorage.password;
        }

        public bool IsPasswordValid(uint id, string password)
        {
            if (!_passwords.ContainsKey(id))
            {
                return false;
            }

            long currentEpoch = _timeManager.CurrentEpochMS;
            PasswordStorage passwordStorage = _passwords[id];

            bool ret = IsTimeValid(passwordStorage.epochCreated, currentEpoch) && passwordStorage.password == password;

            // As the requirements says this is a one-time password, remove it now
            _passwords.Remove(id);

            return ret;
        }

        public long TimeValidity
        {
            get { return PASSWORD_VALID_MS; }
        }
        //@}


        /// <summary>
        /// Gets the current number of passwords stored in the manager
        /// </summary>
        public int NumPasswordsStored
        {
            get { return _passwords.Count; }
        }

        /// <summary>
        /// Deletes the passwords with expired time from the manager
        /// </summary>
        public void RemoveExpiredPasswords()
        {
            long currentEpoch = _timeManager.CurrentEpochMS;

            foreach (var ps in _passwords.ToList())
            {
                if (ps.Value.epochCreated + PASSWORD_VALID_MS > currentEpoch)
                    _passwords.Remove(ps.Key);
            }
        }

        // Checks if time between created and current is inside the curren limits for password validity
        bool IsTimeValid(long created, long current)
        {
            long epochDiff = current - created;
            return epochDiff >= 0 && epochDiff <= PASSWORD_VALID_MS;
        }

        ITimeManager _timeManager; // Implementation of ITimeManaget to be user by this instance
        Random _random = new Random(); // Aux object for password creation. In a real implementation it must
                                       // be a more complex system, probably base on salt+hash algorithms

        // The current class that the manager stores for each ID+password
        class PasswordStorage
        {
            public string password; // Password for current user
            public long epochCreated; // Time when the password was created
        }

        Dictionary<uint, PasswordStorage> _passwords = new Dictionary<uint, PasswordStorage>(); // Stores the passwords
        const long PASSWORD_VALID_MS = 30 * 1000; // Max validity of passwords: 30 seconds
    }
}
