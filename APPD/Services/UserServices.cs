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
        private const string fileURI = "pack://application:,,,/Assets/SimulatedServer/Database/Users.json";

        private class UsersJSONObjectBridge
        {
            public List<User> Users { get; set; }
        }

        public static List<User> getUsersFromDatabase()
        {
            UsersJSONObjectBridge deserializedObject;
            bool garbageCleared = false;
            using (StreamReader reader = new StreamReader(Application.GetContentStream(new Uri(fileURI)).Stream))
            {
                string usersJsonFileContents = reader.ReadToEnd();

                deserializedObject = JsonConvert.DeserializeObject<UsersJSONObjectBridge>(usersJsonFileContents);
                
                foreach(User u in deserializedObject.Users)
                {
                    foreach(AccountRentalData ard in u.AccountsRented)
                    {
                        ard.DaysRented = ard.DaysRented.Where(x => DateTime.Today < x.AddDays(1)).ToList();
                        garbageCleared = true;
                    }
                }
            }

            if (garbageCleared)
            {
                writeUsersDatabase(JsonConvert.SerializeObject(
                    new UsersJSONObjectBridge { Users = deserializedObject.Users }));
            }

            return deserializedObject.Users;
        }

        public static void writeUsersDatabase(string json)
        {
            using (FileStream stream = new FileStream((new Uri(fileURI)).AbsolutePath.Substring(1), FileMode.Create))
            {
                // Overwrite the file
                stream.SetLength(0);

                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(json);
                }
            }
        }

        public static User LogIn(string username, string password)
        {
            var Users = getUsersFromDatabase();
            foreach(User user in Users)
                if (user.Username == username && user.Password == password)
                    return user;

            return null;
        }

        // Returns true if successful, false otherwise.
        public static User PerformRental(List<DateTime> dates, int accountID, User user)
        {
            List<Account> accounts = AccountServices.getAccountsFromDatabase();
            List<User> users = getUsersFromDatabase();

            User transactingUser = user;
            if (transactingUser == null)
                return null;

            AccountRentalData rentalData = transactingUser.AccountsRented.Where(x => x.ID == accountID)
                                            .ToList().ElementAtOrDefault(0);
            if (rentalData == null)
            {
                AccountRentalData newRentalData = new AccountRentalData
                {
                    DaysRented = new List<DateTime>(),
                    ID = accountID
                };
                transactingUser.AccountsRented.Add(newRentalData);
                rentalData = newRentalData;
            }

            rentalData.DaysRented.AddRange(dates);

            Account rentedAccount = accounts.Where(acc => acc.ID == accountID).ToList().ElementAtOrDefault(0);
            if (rentedAccount == null)
                return null;

            transactingUser.Dankness -= rentedAccount.DanknessPerDay * dates.Count;

            int accountOwnerID = rentedAccount.getAuthor().ID;

            User accountOwner = users.Where(u => u.ID == accountOwnerID).ElementAtOrDefault(0);
            if (accountOwner == null)
                return null;

            accountOwner.Dankness += rentedAccount.DanknessPerDay * dates.Count;

            UsersJSONObjectBridge usersBridge = new UsersJSONObjectBridge() { Users = users };

            string jsonStr = JsonConvert.SerializeObject(usersBridge);

            writeUsersDatabase(jsonStr);

            return transactingUser;
        }
    }
}
