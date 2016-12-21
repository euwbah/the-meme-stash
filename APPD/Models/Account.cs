using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPD.Models
{
    public class Account
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public int DanknessPerDay { get; private set; }

        public List<AccountCredential> Credentials;

        public Account(int ID, string Name, string Description, int DanknessPerDay, string imagePath, List<AccountCredential> Credentials)
        {
            this.ID = ID;
            this.Name = Name;
            this.Description = Description;
            this.DanknessPerDay = DanknessPerDay;
            this.Credentials = Credentials;
            this.ImagePath = imagePath;
        }
    }
}
