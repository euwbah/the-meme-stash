using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPD.Models
{

    internal delegate void DanknessUpdateHandler(User sender);

    public class User
    {
        private int _dankness;

        public int ID { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public List<AccountRentalData> AccountsRented { get; private set; }
        public List<int> AccountsForRent { get; private set; }

        public int Dankness
        {
            get { return _dankness; }
            private set
            {
                if (_dankness != value)
                {
                    _dankness = value;
                    OnDanknessUpdated?.Invoke(this);
                }
            }
        }

        public string ProfilePicPath { get; private set; }
        public string Bio { get; private set; }

        internal event DanknessUpdateHandler OnDanknessUpdated;

        public User(int id, string username, string password, List<AccountRentalData> accountsRented, 
                    List<int> accountsForRent, int dankness, string bio)
        {
            this.ID = id;
            this.Username = username;
            this.Password = password;
            this.AccountsRented = accountsRented;
            this.AccountsForRent = accountsForRent;
            this.Dankness = dankness;
            this.ProfilePicPath = this.generateProfilePicPath();
            this.Bio = bio;
        }

        private string generateProfilePicPath()
        {
            return "pack://application:,,,/Assets/SimulatedServer/Assets/Images/" + ID + ".png";
        }
    }
}
