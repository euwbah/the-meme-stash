using APPD.Services;
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
            internal set
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


        #region JSON.NET metas
        public bool ShouldSerializeProfilePicPath() { return false; }
        #endregion

        internal event DanknessUpdateHandler OnDanknessUpdated;

        public User(int ID, string Username, string Password, List<AccountRentalData> AccountsRented, 
                    List<int> AccountsForRent, int Dankness, string Bio)
        {
            this.ID = ID;
            this.Username = Username;
            this.Password = Password;
            this.AccountsRented = AccountsRented;
            this.AccountsForRent = AccountsForRent;
            this.Dankness = Dankness;
            this.ProfilePicPath = this.generateProfilePicPath();
            this.Bio = Bio;
        }

        public User PerformRental(List<DateTime> dates, Account account)
        {
            return UserServices.PerformRental(dates, account.ID, this);
        }

        private string generateProfilePicPath()
        {
            return "pack://application:,,,/Assets/SimulatedServer/Assets/Images/Users/" + ID + ".png";
        }
    }
}
