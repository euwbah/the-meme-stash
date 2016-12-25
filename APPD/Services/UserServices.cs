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

            //Users.Add(new User(0, "PapaDanku", "pinkisawesome",
            //    new List<AccountRentalData>(), new List<int>(), 1337, "yamete"));

            //Users.Add(new User(1, "theLegend27", "capitalism",
            //    new List<AccountRentalData>(), new List<int>(), 9001, "Who is the legend 27?"));

            //Users.Add(new User(2, "XXXemokidXXX", "123456",
            //    new List<AccountRentalData>(), new List<int>(), 60, "nigga please"));

            //// Make life easier (temporary account)
            //Users.Add(new User(3, "a", "a",
            //    new List<AccountRentalData>
            //    {
            //        new AccountRentalData {
            //            DaysRented = new List<DateTime> {
            //                DateTime.Today,          // This is for debugging purposes
            //                new DateTime(2017, 1, 3) // Assignment DUE!!!!!
            //            },
            //            ID = 0
            //        }
            //    },
            //    new List<int>(), 60, "nigga please"));

            //return Users;
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
