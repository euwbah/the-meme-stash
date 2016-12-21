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

        public static List<User> readUsersFromDatabase()
        {
            // This is just a placeholder for the JSON database

            List<User> Users = new List<User>();

            Users.Add(new User(0, "PapaDanku", "pinkisawesome",
                new List<AccountRentalData>(), new List<int>(), 1337, "yamete"));

            Users.Add(new User(1, "theLegend27", "capitalism",
                new List<AccountRentalData>(), new List<int>(), 9001, "Who is the legend 27?"));

            Users.Add(new User(2, "XXXemokidXXX", "123456",
                new List<AccountRentalData>(), new List<int>(), 60, "nigga please"));

            // Make life easier (temporary account)
            Users.Add(new User(3, "a", "a",
                new List<AccountRentalData>(), new List<int>(), 60, "nigga please"));

            return Users;
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
