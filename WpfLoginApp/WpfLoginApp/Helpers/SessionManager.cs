using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfLoginApp.Properties;

namespace WpfLoginApp.Helpers
{
    public static class SessionManager
    {
        private static readonly int SessionTimer = 10; // change as needed
        public static void SaveSession(string username)
        {
            Settings.Default.Username = username;
            Settings.Default.IsLoggedIn = true;
            Settings.Default.LoginTime = DateTime.Now;

            Settings.Default.Save();
        }

        public static string GetSession()
        {
            return Settings.Default.IsLoggedIn
                 ? Settings.Default.Username
                 : null;
        }

        public static void ClearSession()
        {
            Settings.Default.Username = "";
            Settings.Default.IsLoggedIn = false;
            Settings.Default.LoginTime = DateTime.MinValue;

            Settings.Default.Save();
        }

        public static bool IsLoggedIn()
        {
            if (!Settings.Default.IsLoggedIn)
                return false;

            var loginTime = Settings.Default.LoginTime;

            var expiryTime = loginTime.AddHours(SessionTimer);

            if (DateTime.Now > expiryTime)
            {
                ClearSession();
                return false;
            }

            return true;
        }
    }
}
