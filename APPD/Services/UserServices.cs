﻿using APPD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPD.Services
{
    public static class UserServices
    {
        private static List<User> _localDatabaseOfUsers;

        private static List<User> localDatabaseOfUsers
        {
            get
            {
                UpdateLocalDatabaseOfUsers();
                return _localDatabaseOfUsers;
            }
            set { _localDatabaseOfUsers = value; }
        }

        public static void UpdateLocalDatabaseOfUsers()
        {
            // This is just a placeholder for the JSON database

            _localDatabaseOfUsers = new List<User>();

            _localDatabaseOfUsers.Add(new User("PapaDanku", "pinkisawesome",
                new List<AccountRentalData>(), new List<Account>(), 1337));

            _localDatabaseOfUsers.Add(new User("theLegend27", "capitalism",
                new List<AccountRentalData>(), new List<Account>(), 9001));

            _localDatabaseOfUsers.Add(new User("XXXemokidXXX", "123456",
                new List<AccountRentalData>(), new List<Account>(), 60));
        }

        public static bool LogIn(string username, string password)
        {
            foreach(User user in localDatabaseOfUsers)
                if (user.Username == username && user.Password == password)
                    return true;

            return false;
        }
    }
}
