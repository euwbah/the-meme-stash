using APPD.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace APPD.Services
{
    public static class UserServices
    {

        private class UsersJSONObjectBridge
        {
            public List<User> Users { get; set; }
        }

        public static List<User> readUsersFromDatabase()
        {
            // This is just a placeholder for the JSON database

            string fileURI = "pack://application:,,,/Assets/SimulatedServer/Database/Users.json";
            
            StreamReader reader = new StreamReader(Application.GetResourceStream(new Uri(fileURI)).Stream);

            string usersJsonFileContents = reader.ReadToEnd();

            UsersJSONObjectBridge deserializedObject = 
                JsonConvert.DeserializeObject<UsersJSONObjectBridge>(usersJsonFileContents);

            return deserializedObject.Users;
        }

        public static User LogIn(string username, string password)
        {
            var Users = readUsersFromDatabase();
            foreach(User user in Users)
                if (user.Username == username && user.Password == password)
                    return user;

            return null;
        }
    }
}
