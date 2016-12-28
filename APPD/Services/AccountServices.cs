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

        public static List<Account> getAccountsFromDatabase()
        {
            string fileURI = "pack://application:,,,/Assets/SimulatedServer/Database/Accounts.json";
            StreamReader reader = new StreamReader(Application.GetContentStream(new Uri(fileURI)).Stream);
            string accountsJsonFileContents = reader.ReadToEnd();

            AccountsJSONObjectBridge deserializedObject =
                JsonConvert.DeserializeObject<AccountsJSONObjectBridge>(accountsJsonFileContents);

            return deserializedObject.Accounts;
        }

        

        internal static User getAuthor(this Account account)
        {
            List<User> users = UserServices.getUsersFromDatabase();
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

        internal static List<Account> getAccountsOwnedBy(User user)
        {
            return 
                (
                from a in getAccountsFromDatabase()
                where a.Author.ID == user.ID
                orderby a.Name ascending
                select a
            ).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns> Returns all accounts presently booked by the user. </returns>
        internal static List<Account> getAccountsRentedBy(User user)
        {
            return (
                from a in getAccountsFromDatabase()
                where user.AccountsRented.Select(ard => ard.ID).Contains(a.ID)
                select a
                ).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="takeAccountsCurrentlyInPossession"></param>
        /// <returns>
        /// Returns all accounts presently booked, and either in possession, or otherwise,
        /// depending on the <code>takeAccountsCurrentlyInPossession</code> parameter.
        /// </returns>
        internal static List<Account> getAccountsRentedBy(User user, bool takeAccountsCurrentlyInPossession)
        {
            List<int> accountIDsWhereAccountIsCurrentlyInUsersPossession = 
                user.AccountsRented.Where(ard =>
                ard.DaysRented.Where(dateTime => dateTime.Date == DateTime.Today).Count() != 0)
                .Select(x => x.ID).ToList();

            return getAccountsRentedBy(user).Where(x =>
            {
                bool pred = accountIDsWhereAccountIsCurrentlyInUsersPossession.Contains(x.ID);
                return takeAccountsCurrentlyInPossession ? pred : !pred;
            }).ToList();
        }

        internal static List<Account> getFeaturedAccounts()
        {
            List<Account> accounts = getAccountsFromDatabase();
            return accounts.Where(account => account.IsFeatured)
                           .ToList();
        }

        // Note the nthNewest is 0 index
        internal static Account getNthNewestAccount(int nthNewest)
        {
            List<Account> featuredAccounts = getFeaturedAccounts();
            List<Account> accounts = getAccountsFromDatabase();
            return accounts.OrderByDescending(account => account, new AccountDateComparer()).ToList().ElementAtOrDefault(nthNewest);
        }

        internal static List<AccountRentalData> getAccountRentalDataList(this Account account)
        {
            List<AccountRentalData> returnable = new List<AccountRentalData>();

            List<User> users = UserServices.getUsersFromDatabase();
            
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

            List<User> users = UserServices.getUsersFromDatabase();
            foreach(User user in users)
            {
                foreach(AccountRentalData ard in user.AccountsRented)
                {
                    if (ard.ID == account.ID)
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
