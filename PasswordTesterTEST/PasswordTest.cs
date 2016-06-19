using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PasswordTesterAPI;

namespace PasswordTester
{
    [TestClass]
    public class PasswordTest
    {
        [TestMethod]
        public void TestMinMaxIDs()
        {
            var mockTime = new MockTimeManager();
            var passwordManager = new PasswordManager(mockTime);

            Assert.IsTrue(!String.IsNullOrEmpty(passwordManager.CreatePassword(0)));
            Assert.IsTrue(!String.IsNullOrEmpty(passwordManager.CreatePassword(uint.MinValue)));
            Assert.IsTrue(!String.IsNullOrEmpty(passwordManager.CreatePassword(uint.MaxValue)));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Must set a valid TimeManager implementation")]
        public void TestTimeManagerNotNull()
        {
            var passwordManager = new PasswordManager(null);
        }

        [TestMethod]
        public void TestOneTimeValidity()
        {
            var mockTime = new MockTimeManager();
            var passwordManager = new PasswordManager(mockTime);

            Assert.IsTrue(passwordManager.NumPasswordsStored == 0);
            string pass1 = passwordManager.CreatePassword(1);
            Assert.IsTrue(passwordManager.NumPasswordsStored == 1);
            Assert.IsTrue(passwordManager.IsPasswordValid(1, pass1));
            Assert.IsTrue(passwordManager.NumPasswordsStored == 0);
            Assert.IsFalse(passwordManager.IsPasswordValid(1, pass1));
        }

        [TestMethod]
        public void TestTimeValidity()
        {
            var mockTime = new MockTimeManager();
            var passwordManager = new PasswordManager(mockTime);

            // Check current time prior to pass creation
            mockTime.Time = 1000;
            string pass1 = passwordManager.CreatePassword(1);

            mockTime.Time -= 1;
            Assert.IsFalse(passwordManager.IsPasswordValid(1, pass1));

            // Check max valid time
            mockTime.Time = 1000;
            pass1 = passwordManager.CreatePassword(1);

            mockTime.Time += passwordManager.TimeValidity;
            Assert.IsTrue(passwordManager.IsPasswordValid(1, pass1));

            // Check max valid time plus 1 ms
            mockTime.Time = 1000;
            pass1 = passwordManager.CreatePassword(1);

            mockTime.Time += passwordManager.TimeValidity + 1;
            Assert.IsFalse(passwordManager.IsPasswordValid(1, pass1));

            // Check time validity increment when generating password to the same user
            mockTime.Time = 1000;
            pass1 = passwordManager.CreatePassword(1);
            mockTime.Time += passwordManager.TimeValidity;
            string pass2 = passwordManager.CreatePassword(1);
            Assert.AreEqual(pass1, pass2);

            mockTime.Time += passwordManager.TimeValidity;
            Assert.IsTrue(passwordManager.IsPasswordValid(1, pass1));

            // Check creating a new pass when time is expired gives a different pass
            mockTime.Time = 1000;
            pass1 = passwordManager.CreatePassword(1);
            mockTime.Time += passwordManager.TimeValidity + 1;
            pass2 = passwordManager.CreatePassword(1);
            Assert.AreNotEqual(pass1, pass2);

            mockTime.Time += passwordManager.TimeValidity;
            Assert.IsTrue(passwordManager.IsPasswordValid(1, pass2));
        }

        [TestMethod]
        public void TestExpiredPasswordsDeletion()
        {
            var mockTime = new MockTimeManager();
            var passwordManager = new PasswordManager(mockTime);

            mockTime.Time = 1000;
            string pass1 = passwordManager.CreatePassword(1);
            string pass2 = passwordManager.CreatePassword(2);
            string pass3 = passwordManager.CreatePassword(3);

            mockTime.Time += passwordManager.TimeValidity + 1;
            string pass4 = passwordManager.CreatePassword(4);
            string pass5 = passwordManager.CreatePassword(5);

            Assert.IsTrue(passwordManager.NumPasswordsStored == 5);
            passwordManager.RemoveExpiredPasswords();
            Assert.IsTrue(passwordManager.NumPasswordsStored == 3);
        }
    }

    // Mock class for testing time increments and password validity
    class MockTimeManager : ITimeManager
    {
        // From ITimeManager
        //@{
        public long CurrentEpochMS
        {
            get { return _time; }
        }
        //@}

        public long Time
        {
            get { return _time; }
            set { _time = value; }
        }

        long _time;
    }
}
