using System;

namespace PasswordTesterAPI
{
    /// <summary>
    /// Public interface to create and check passwords for a given ID
    /// </summary>
    public interface IPasswordManager
    {
        /// <summary>
        /// Creates a password for a user
        /// </summary>
        /// <param name="id">ID of the user</param>
        /// <returns>Password for the user</returns>
        string CreatePassword(uint id);

        /// <summary>
        /// Checks if the combination of user+password is valid
        /// </summary>
        /// <param name="id">ID of the user</param>
        /// <param name="password">Password to check the validity</param>
        /// <returns>True if the password is valid, false otherwise</returns>
        bool IsPasswordValid(uint id, string password);

        /// <summary>
        /// Valid time for passwords that the current implementation of IPasswordManager uses
        /// </summary>
        long TimeValidity { get; }
    }
}
