using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfLoginApp.Models;

namespace WpfLoginApp.Services
{
    public static class AuthService
    {
        
        private static string folderPath = @"D:\Practise\WpfLoginApp";
        private static string filePath = Path.Combine(folderPath, "users.txt");

        static AuthService()
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
        }

        public static bool Signup(string username, string password)
        {
            var users = GetUsers();

            if (users.Any(x => x.Username == username))
                return false;

            File.AppendAllText(filePath, $"{username},{password}\n");

            return true;
        }

        public static bool Login(string username, string password)
        {
            var users = GetUsers();

            return users.Any(x =>
                x.Username == username &&
                x.Password == password);
        }

        private static List<UserModel> GetUsers()
        {
            var list = new List<UserModel>();

            if (!File.Exists(filePath))
                return list;

            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var parts = line.Split(',');

                list.Add(new UserModel
                {
                    Username = parts[0],
                    Password = parts[1]
                });
            }

            return list;
        }
    }
}
