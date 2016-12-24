using APPD.Models;
using APPD.Services;
using APPD.ViewModels;
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
    public class ConvertDateTime_Account_User_ToCalendarDayBackgroundFillBrush : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // NOTE that this is OK!!
            // Since the styling is beinig done in a control template, the first time the converter is called,
            // the data bindings are not valid yet.

            if (values.Contains(DependencyProperty.UnsetValue))
                return DependencyProperty.UnsetValue;

            DateTime currentDateToColor = (DateTime)values[0];

            AccountViewModel dataContext = (AccountViewModel)values[1];
            if (dataContext == null)
                return DependencyProperty.UnsetValue;

            Account currentAccount = dataContext.CurrentAccount;
            List<AccountRentalData> rentalDatas = dataContext.RentalDataList;
            List<DateTime> currentSelectedDates = dataContext.SelectedDates;
            User user = dataContext.parent.parent.State.CurrentLoggedOnUser;

            if (currentAccount == null || rentalDatas == null || currentSelectedDates == null || user == null)
                return DependencyProperty.UnsetValue;

            SolidColorBrush fillBrush = new SolidColorBrush();

            // Check for dates the current logged on user has already booked the account
            foreach (AccountRentalData ard in user.AccountsRented)
            {
                if (ard.ID == currentAccount.ID && ard.DaysRented.Contains(currentDateToColor))
                {
                    // Color: Calm green
                    fillBrush.Color = Color.FromArgb(0xbb, 0x33, 0x77, 0xff);
                    return fillBrush;
                }
            }

            // Check for dates where other users have booked the account
            foreach (AccountRentalData ard in rentalDatas)
            {
                if(ard.ID == currentAccount.ID && ard.DaysRented.Contains(currentDateToColor))
                {
                    // Color: Orange
                    fillBrush.Color = Color.FromArgb(0xbb, 0xff, 0x99, 0x33);
                    return fillBrush;
                }
            }

            if(currentSelectedDates.Contains(currentDateToColor))
            {
                fillBrush.Color = Color.FromArgb(0xee, 0x11, 0xcc, 0xff);
                return fillBrush;
            }

            // The default appearance: Open
            fillBrush.Color = Color.FromArgb(0xbb, 0x33, 0xff, 0x77);
            return fillBrush;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
