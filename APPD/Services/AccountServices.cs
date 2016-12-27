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
    public static class AccountServices
    {

        private class AccountsJSONObjectBridge
        {
            public List<Account> Accounts { get; set; }
        }

        public static List<Account> readAccountsFromDatabase()
        {
            string fileURI = "pack://application:,,,/Assets/SimulatedServer/Database/Accounts.json";
            StreamReader reader = new StreamReader(Application.GetResourceStream(new Uri(fileURI)).Stream);
            string accountsJsonFileContents = reader.ReadToEnd();

            AccountsJSONObjectBridge deserializedObject =
                JsonConvert.DeserializeObject<AccountsJSONObjectBridge>(accountsJsonFileContents);

            return deserializedObject.Accounts;
        }

        

        internal static User getAuthor(this Account account)
        {
            List<User> users = UserServices.readUsersFromDatabase();
            foreach (User u in users)
            {
                foreach(int accountID in u.AccountsForRent)
                {
                    if (accountID == account.ID)
                        return u;
                }
            }

#if DEBUG
            throw new Exception("Database has errors!");
#endif
            return null;
        }

        internal static List<Account> getFeaturedAccounts()
        {
            List<Account> accounts = readAccountsFromDatabase();
            return accounts.Where(account => account.IsFeatured)
                           .ToList();
        }

        internal static List<Account> getNewAccounts()
        {
            List<Account> featuredAccounts = getFeaturedAccounts();
            List<Account> accounts = readAccountsFromDatabase();
            return accounts.OrderByDescending(account => account, new AccountDateComparer())
                           .Where(account => !featuredAccounts.Contains(account))
                           .Take(3)
                           .ToList();
        }

        internal static List<AccountRentalData> getAccountRentalDataList(this Account account)
        {
            List<AccountRentalData> returnable = new List<AccountRentalData>();

            List<User> users = UserServices.readUsersFromDatabase();
            
            foreach(User u in users)
                returnable.AddRange(u.AccountsRented.Where(acc => acc.ID == account.ID));

            return returnable;
        }

        // This is more of a helper function than a service, but anyways...
        internal static List<AccountRentalData> getAccountRentalDataList(this Account account, User fromUser)
        {
            List<AccountRentalData> returnable = new List<AccountRentalData>();
            
            returnable.AddRange(fromUser.AccountsRented.Where(acc => acc.ID == account.ID));

            return returnable;
        }

        internal static List<DateTime> getListOfBookableDates(this Account account)
        {
            List<AccountRentalData> rentalDates = getAccountRentalDataList(account);
            List<DateTime> listOfBookableDates = new List<DateTime>();

            for (short daysAfterToday = 0; daysAfterToday <= ConfigServices.MAX_DAYS_BOOKABLE_IN_ADVANCE; daysAfterToday++)
                listOfBookableDates.Add(DateTime.Today.AddDays(daysAfterToday));

            List<User> users = UserServices.readUsersFromDatabase();
            foreach(User user in users)
            {
                foreach(AccountRentalData ard in user.AccountsRented)
                {
                    listOfBookableDates.RemoveAll(date => ard.DaysRented.Contains(date));
                }
            }

            return listOfBookableDates;
        }

        private class AccountDateComparer : IComparer<Account>
        {
            public int Compare(Account x, Account y)
            {
                if (x.AccountCreationDate > y.AccountCreationDate)
                    return 1;
                else if (x.AccountCreationDate < y.AccountCreationDate)
                    return -1;
                else
                    return 0;
            }
        }
    }
}
