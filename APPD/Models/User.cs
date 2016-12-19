using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPD.Models
{
    public class User
    {

        public string Username { get; private set; }
        public string Password { get; private set; }
        public List<AccountRentalData> AccountsRented { get; private set; }
        public List<int> AccountsForRent { get; private set; }
        public int Dankness { get; private set; }

        public User(string username, string password, List<AccountRentalData> accountsRented, 
                    List<int> accountsForRent, int dankness)
        {
            this.Username = username;
            this.Password = password;
            this.AccountsRented = accountsRented;
            this.AccountsForRent = accountsForRent;
            this.Dankness = dankness;
        }
    }
}
