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
                    new List<AccountCredential> { })
            };

            return accounts;
        }

        internal static List<Account> getFeaturedAccounts()
        {
            List<Account> accounts = getAccountsFromDatabase();
            return accounts.Where(account => account.IsFeatured).ToList();
        }

        internal static List<Account> getNewAccounts()
        {
            List<Account> accounts = getAccountsFromDatabase();
            return accounts.OrderByDescending();
        }
    }
}
