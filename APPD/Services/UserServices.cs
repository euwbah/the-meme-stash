using APPD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPD.Services
{
    public static class UserServices
    {
        private static List<User> _users;

        private static List<User> Users
        {
            get
            {
                readUsersFromDatabase();
                return _users;
            }
        }

        public static void readUsersFromDatabase()
        {
            // This is just a placeholder for the JSON database

            _users = new List<User>();

            _users.Add(new User(0, "PapaDanku", "pinkisawesome",
                new List<AccountRentalData>(), new List<int>(), 1337, "yamete"));

            _users.Add(new User(1, "theLegend27", "capitalism",
                new List<AccountRentalData>(), new List<int>(), 9001, "Who is the legend 27?"));

            _users.Add(new User(2, "XXXemokidXXX", "123456",
                new List<AccountRentalData>(), new List<int>(), 60, "nigga please"));

            // Make life easier (temporary account)
            _users.Add(new User(3, "a", "a",
                new List<AccountRentalData>(), new List<int>(), 60, "nigga please"));
        }

        public static User LogIn(string username, string password)
        {
            foreach(User user in Users)
                if (user.Username == username && user.Password == password)
                    return user;

            return null;
        }
    }
}
