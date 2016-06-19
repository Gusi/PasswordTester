using System;
using PasswordTesterAPI;

namespace PasswordTester
{
    // Class to demonstrate the usage of the PasswordTesterAPI
    static class PasswordTester
    {
        static TimeManager sTimeManager = new TimeManager();
        static PasswordManager sPasswordManager = new PasswordManager(sTimeManager);

        public static void Run()
        {
            string line = String.Empty;
            do
            {
                Console.WriteLine("\nOPTIONS");
                Console.WriteLine("  1 to create a new password for user");
                Console.WriteLine("  2 to check validity of password");
                Console.WriteLine("  3 to exit");
                Console.WriteLine("\nEnter your choice");
                line = Console.ReadLine();
                switch (line)
                {
                    case "1":
                        DoCreateNewPassword();
                        break;
                    case "2":
                        DoVerifyPassword();
                        break;
                    case "3":
                        // Let loop exit at iteration end
                        break;
                    default:
                        // Print menu again
                        break;
                }
            } while (line != "3");
        }

        static void DoCreateNewPassword()
        {
            uint? id = ParseUser();
            if (id == null)
                return;

            string pass = sPasswordManager.CreatePassword(id.Value);
            Console.WriteLine("Password for user " + id + " is '" + pass + "'");
        }

        static void DoVerifyPassword()
        {
            uint? id = ParseUser();
            if (id == null)
                return;

            Console.WriteLine("Enter password (empty to return)");
            string password = Console.ReadLine();
            if (String.IsNullOrEmpty(password))
                return;

            bool valid = sPasswordManager.IsPasswordValid(id.Value, password.ToUpper());
            Console.WriteLine("Password '" + password + "' for user " + id.Value + " is " + (valid ? "VALID" : "INVALID"));
        }

        static uint? ParseUser()
        {
            string user = String.Empty;

            do
            {
                Console.WriteLine("\nEnter user ID (unsigned integer; empty to return)");
                user = Console.ReadLine();

                uint id;
                if (uint.TryParse(user, out id))
                    return id;
            } while (!String.IsNullOrEmpty(user));

            return null;
        }
    }
}
