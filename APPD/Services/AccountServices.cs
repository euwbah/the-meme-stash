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
                new Account(0, "Pakalu Papito", "I am a camel", 40,
                    new List<AccountCredential> { })
            };

            return accounts;
        }
    }
}
