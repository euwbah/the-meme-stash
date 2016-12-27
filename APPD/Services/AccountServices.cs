using APPD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPD.Services
{
    public static class AccountServices
    {
        internal static List<Account> getAccountsFromDatabase()
        {
            List<Account> accounts = new List<Account>()
            {
                new Account(0, "asdfghj", "aergair", 40, new DateTime(2013, 9, 11), true,
                    new List<AccountCredential> {
                        new AccountCredential("9GAG", "asdfghj", "asdfghj@asdfmovies.com", "asdffdsa"),
                        new AccountCredential("4CHAN", "asdfghj", null, "wefjiwoefj")
                    },
                    new List<string> {
                        "nigga", "bitch"
                    }),
                new Account(1, "asdf", "iroauergb", 90, new DateTime(2015, 10, 11), false,
                    new List<AccountCredential> { },
                    new List<string> {
                        "fuckface"
                    }),
                new Account(2, "qwerty", "aowvnoeairn", 30, new DateTime(2016, 10, 11), false,
                    new List<AccountCredential> { },
                    new List<string> {
                        "bitch", "nigga ass"
                    })
            };

            return accounts;
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
            List<Account> accounts = getAccountsFromDatabase();
            return accounts.Where(account => account.IsFeatured)
                           .ToList();
        }

        internal static List<Account> getNewAccounts()
        {
            List<Account> featuredAccounts = getFeaturedAccounts();
            List<Account> accounts = getAccountsFromDatabase();
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
