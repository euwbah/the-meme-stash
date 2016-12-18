using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPD.Models
{
    public class User
    {
        private string _username;
        private string _password;
        private List<AccountRentalData> _accountsRented;
        private List<Account> _accountsForRent;
        private int _dankness;

        public string Username { get; private set; }
        public string Password { get; private set; }
        public List<AccountRentalData> AccountsRented { get; private set; }
        public List<Account> AccountsForRent { get; private set; }
        public int Dankness { get; private set; }

        public User(string username, string password, List<AccountRentalData> accountsRented, 
                    List<Account> accountsForRent, int dankness)
        {
            this.Username = username;
            this.Password = password;
            this.AccountsRented = accountsRented;
            this.AccountsForRent = accountsForRent;
            this.Dankness = dankness;
        }
    }
}
