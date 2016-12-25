using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace APPD.Models
{
    public class AccountRentalData
    {
        public int ID { get; set; } //Referring to Account's ID
        public List<DateTime> DaysRented { get; set; }
    }
}
