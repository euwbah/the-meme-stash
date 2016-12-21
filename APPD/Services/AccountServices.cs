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
                new Account(0, "Pakalu Papito", "I am a camel", 40, new DateTime(2013, 9, 11), true,
                    new List<AccountCredential> { }),
                new Account(1, "Chublak Punani", "Laro puti", 50, new DateTime(2015, 10, 11), false,
                    new List<AccountCredential> { }),
                new Account(2, "Black Guy 69", "Punani", 30, new DateTime(2016, 10, 11), false,
                    new List<AccountCredential> { })
            };

            return accounts;
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
