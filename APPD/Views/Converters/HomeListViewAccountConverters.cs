using APPD.Models;
using APPD.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace APPD.Views.Converters
{
    public class ConvertAccountToVisibility : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            // The infolabel will only be visible if the account is owned, booked, fully booked, or active.
            Account account = (Account)value[0];
            User loggedOnUser = (User)value[1];

            // Fully booked
            if (account.getListOfBookableDates().Count == 0)
                return Visibility.Visible;

            // Owned
            else if (AccountServices.getAccountsOwnedBy(loggedOnUser)
                        .Where(acc => acc.ID == account.ID).Count() != 0)
                return Visibility.Visible;

            // Booked / Active
            else if (AccountServices.getAccountsRentedBy(loggedOnUser)
                        .Where(acc => acc.ID == account.ID).Count() != 0)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ConvertAccountToContent : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            // The infolabel will only be visible if the account is owned, booked, fully booked, or active.
            Account account = (Account)value[0];
            User loggedOnUser = (User)value[1];

            // Fully booked
            if (account.getListOfBookableDates().Count == 0)
                return "SOLD OUT";

            // Owned
            else if (AccountServices.getAccountsOwnedBy(loggedOnUser)
                        .Where(acc => acc.ID == account.ID).Count() != 0)
                return "OWNED";

            // Active
            else if (AccountServices.getAccountsRentedBy(loggedOnUser, true)
                        .Where(acc => acc.ID == account.ID).Count() != 0)
                return "ACTIVE";

            // Booked
            else if (AccountServices.getAccountsRentedBy(loggedOnUser, false)
                        .Where(acc => acc.ID == account.ID).Count() != 0)
                return "BOOKED";

            return "";
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ConvertAccountToBackground : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            // The infolabel will only be visible if the account is owned, booked, fully booked, or active.
            Account account = (Account)value[0];
            User loggedOnUser = (User)value[1];

            // Fully booked
            if (account.getListOfBookableDates().Count == 0)
                return new SolidColorBrush(Color.FromArgb(0xff, 0xbb, 0x33, 0x11));

            // Owned
            else if (AccountServices.getAccountsOwnedBy(loggedOnUser)
                        .Where(acc => acc.ID == account.ID).Count() != 0)
                return new SolidColorBrush(Color.FromArgb(0xff, 0x22, 0xcc, 0x55));

            // Active
            else if (AccountServices.getAccountsRentedBy(loggedOnUser, true)
                        .Where(acc => acc.ID == account.ID).Count() != 0)
                return new SolidColorBrush(Color.FromArgb(0xff, 0x88, 0xaa, 0x22));

            // Booked
            else if (AccountServices.getAccountsRentedBy(loggedOnUser, false)
                        .Where(acc => acc.ID == account.ID).Count() != 0)
                return new SolidColorBrush(Color.FromArgb(0xff, 0x11, 0x33, 0x99));

            return new SolidColorBrush(Color.FromArgb(0x00, 0x00, 0x00, 0x00));
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
