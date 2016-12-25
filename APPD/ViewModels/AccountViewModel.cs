using APPD.Helpers;
using APPD.Models;
using APPD.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace APPD.ViewModels
{
    public class AccountViewModel : ObservableObject
    {
        public HomeViewModel parent { get; private set; }

        private Account _currentAccount;
        private List<AccountRentalData> _rentalDataList;
        private Visibility _accountCredentialsVisibility;

        private Visibility _ownedLabelVisibility;
        private string _additionalInformation;

        private string _actionButtonContent;
        private Brush _actionButtonForeground;
        private Brush _actionButtonBackground;

        private ICommand _actionButtonClick;
        private ICommand _overlayCloseClick;
        private ICommand _backButtonClick;

        private int _calendarOverlayZIndex;
        
        // For manually handling the coloring of the calendar
        private List<DateTime> _selectedDates;

        private bool _tooManyDaysMonostable;

        public Account CurrentAccount
        {
            get { return _currentAccount; }
            set
            {
                if (_currentAccount != value)
                {
                    _currentAccount = value;
                    OnPropertyChanged("CurrentAccount");
                    OnPropertyChanged("CurrentAccountCredentials");
                    OnPropertyChanged("DaysBookedByCurrentUser");
                    this.updateProperties();
                }
            }
        }
        public List<AccountCredential> CurrentAccountCredentials
        {
            get { return CurrentAccount?.Credentials; }
        }
        public List<AccountRentalData> RentalDataList
        {
            get { return _rentalDataList; }
            set
            {
                if (value != _rentalDataList)
                {
                    _rentalDataList = value;
                    OnPropertyChanged("RentalDataList");
                }
            }
        }
        public Visibility AccountCredentialsVisibility
        {
            get { return _accountCredentialsVisibility; }
            set
            {
                if (value != _accountCredentialsVisibility)
                {
                    _accountCredentialsVisibility = value;
                    OnPropertyChanged("AccountCredentialsVisibility");
                }
            }
        }

        public Visibility OwnedLabelVisibility
        {
            get { return _ownedLabelVisibility; }
            set
            {
                if (value != _ownedLabelVisibility)
                {
                    _ownedLabelVisibility = value;
                    OnPropertyChanged("OwnedLabelVisibility");
                }
            }
        }
        public string AdditionalInformation
        {
            get { return _additionalInformation; }
            set
            {
                if (value != _additionalInformation)
                {
                    _additionalInformation = value;
                    OnPropertyChanged("AdditionalInformation");
                }
            }
        }
        public string ActionButtonContent
        {
            get { return _actionButtonContent; }
            set
            {
                if(value != _actionButtonContent)
                {
                    _actionButtonContent = value;
                    OnPropertyChanged("ActionButtonContent");
                }
            }
        }
        public Brush ActionButtonForeground
        {
            get { return _actionButtonForeground; }
            set
            {
                if (value != _actionButtonForeground)
                {
                    _actionButtonForeground = value;
                    OnPropertyChanged("ActionButtonForeground");
                }
            }
        }
        public Brush ActionButtonBackground
        {
            get { return _actionButtonBackground; }
            set
            {
                if (value != _actionButtonBackground)
                {
                    _actionButtonBackground = value;
                    OnPropertyChanged("ActionButtonBackground");
                }
            }
        }

        public ICommand ActionButtonClick
        {
            get
            {
                if (_actionButtonClick == null)
                {
                    _actionButtonClick = new RelayCommand(calendarInstance => 
                        this.RentButtonClick((Calendar) calendarInstance));
                }

                return _actionButtonClick;
            }
        }
        public ICommand OverlayCloseClick
        {
            get
            {
                if (_overlayCloseClick == null)
                {
                    _overlayCloseClick = new RelayCommand(param => this.overlayCloseClicked());
                }

                return _overlayCloseClick;
            }
        }
        public ICommand BackButtonClick
        {
            get
            {
                if (_backButtonClick == null)
                {
                    _backButtonClick = new RelayCommand(param => this.backButtonClicked());
                }

                return _backButtonClick;
            }
        }

        // Set this greater than 1 to show
        public int CalendarOverlayZIndex
        {
            get { return _calendarOverlayZIndex; }
            set
            {
                if (value != _calendarOverlayZIndex)
                {
                    _calendarOverlayZIndex = value;
                    OnPropertyChanged("CalendarOverlayZIndex");
                }
            }
        }

        public List<DateTime> SelectedDates
        {
            get { return _selectedDates; }
            set
            {
                if (value != _selectedDates)
                {
                    _selectedDates = value;
                    OnPropertyChanged("SelectedDates");
                }
            }
        }

        public int DaysBookedByCurrentUser
        {
            get
            {
                if (CurrentAccount != null && SelectedDates != null)
                {
                    int counter = 0;
                    CurrentAccount.getAccountRentalDataList(fromUser: parent.parent.State.CurrentLoggedOnUser)
                        .ForEach(ard => counter += ard.DaysRented.Count);

                    return counter + SelectedDates.Count;
                }
                else
                    return 0;
            }
        }
        public int MaxDaysBookablePerAccountPerUser
        {
            get
            {
                return StateServices.MAX_DAYS_BOOKABLE_PER_ACCOUNT_PER_USER;
            }
        }

        public bool TooManyDaysMonostable
        {
            get { return _tooManyDaysMonostable; }
            set
            {
                if (value != _tooManyDaysMonostable)
                {
                    _tooManyDaysMonostable = value;
                    OnPropertyChanged("TooManyDaysMonostable");
                }
            }
        }

        // Not using the Button.IsEnabled property to h4x custom styling.
        private bool isActionButtonActivated;

        public AccountViewModel(HomeViewModel parent)
        {
            this.parent = parent;
        }

        // The init function, basically
        private void updateProperties()
        {
            this.SelectedDates = new List<DateTime>();
            this.RentalDataList = CurrentAccount.getAccountRentalDataList();
            User currentLoggedOnUser = parent.parent.State.CurrentLoggedOnUser;
            List<DateTime> userAlreadyConfirmedBookingDates = new List<DateTime>();
            CalendarOverlayZIndex = -1;

            CurrentAccount.getAccountRentalDataList(fromUser: currentLoggedOnUser).ForEach(
                ard => userAlreadyConfirmedBookingDates.AddRange(ard.DaysRented)
            );

            if (userAlreadyConfirmedBookingDates.Contains(DateTime.Today))
                AccountCredentialsVisibility = Visibility.Visible;
            else
                AccountCredentialsVisibility = Visibility.Collapsed;

            if (currentLoggedOnUser.AccountsForRent.Contains(this.CurrentAccount.ID))
            {
                // Current Account belongs to the current logged on user

                OwnedLabelVisibility = Visibility.Visible;
                ActionButtonContent = "EDIT";
                ActionButtonForeground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                ActionButtonBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x99, 0x66, 0x11));

                isActionButtonActivated = false;
            }
            else if (userAlreadyConfirmedBookingDates.Count == StateServices.MAX_DAYS_BOOKABLE_PER_ACCOUNT_PER_USER)
            {
                // User has reach max days bookable per account quote

                OwnedLabelVisibility = Visibility.Hidden;
                ActionButtonContent = "MAXED OUT";
                AdditionalInformation = "You currently have the maximum of " +
                                        StateServices.MAX_DAYS_BOOKABLE_PER_ACCOUNT_PER_USER + 
                                        " days booked. Check out the other accounts we have first, " +
                                        " and come back again on another day.";
                ActionButtonForeground = new SolidColorBrush(Color.FromArgb(0xBB, 0xFF, 0xFF, 0xFF));
                ActionButtonBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x88, 0x44, 0x44));
                isActionButtonActivated = false;
            }
            else if (CurrentAccount.getListOfBookableDates().Count == 0)
            {
                // Current Account is fully booked

                OwnedLabelVisibility = Visibility.Hidden;
                ActionButtonContent = "NO STOCK";
                AdditionalInformation = "This account is fully booked for the next 30 days. Check again tomorrow!";
                ActionButtonForeground = new SolidColorBrush(Color.FromArgb(0xBB, 0xFF, 0xFF, 0xFF));
                ActionButtonBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x88, 0x44, 0x44));
                isActionButtonActivated = false;
            }
            else
            {
                OwnedLabelVisibility = Visibility.Hidden;
                ActionButtonContent = "RENT NOW";
                ActionButtonForeground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                ActionButtonBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x22, 0x99, 0x44));

                isActionButtonActivated = true;
            }

            updateOverlay();
        }

        private void RentButtonClick(Calendar calendar)
        {
            if (isActionButtonActivated)
            {
                // This part initializes the calendar

                // Updates the calendar's display with h4x
                calendar.DisplayDate = calendar.DisplayDate.AddYears(1);
                calendar.DisplayDate = calendar.DisplayDate.AddYears(-1);

                calendar.DisplayDateStart = DateTime.Today;
                calendar.DisplayDateEnd = DateTime.Today.AddDays(30);

                // Add event handler
                calendar.SelectedDatesChanged += Calendar_SelectedDatesChanged;

                showOverlay();
            }
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Calendar calendar = (Calendar)sender;

            if (calendar.SelectedDate != null)
            {
                DateTime date = calendar.SelectedDate.GetValueOrDefault();
                calendar.SelectedDate = null;
                if (SelectedDates.Contains(date))
                {
                    SelectedDates.Remove(date);
                }
                else
                {
                    if (DaysBookedByCurrentUser < StateServices.MAX_DAYS_BOOKABLE_PER_ACCOUNT_PER_USER)
                    {
                        if (CurrentAccount.getListOfBookableDates().Contains(date))
                            SelectedDates.Add(date);
                    }
                    else
                    {
                        performTooManyDaysAnimation();
                    }
                }

                updateOverlay();
                // h4xh4xh4xh4xh4x
                calendar.DisplayDate = date;
                calendar.DisplayDate = calendar.DisplayDate.AddYears(1);
                calendar.DisplayDate = date;
                calendar.DisplayDate = calendar.DisplayDate.AddYears(-1);
                calendar.DisplayDate = date;
            }
        }

        private void overlayCloseClicked()
        {
            hideOverlay();
            this.SelectedDates.RemoveAll(iHadAnAbsoluteShitChristmas => true);
        }

        private void backButtonClicked()
        {
            parent.Go(from: Screen.ACCOUNT, to: Screen.HOME);
        }

        private void showOverlay() { CalendarOverlayZIndex = 2; updateOverlay(); }
        private void hideOverlay() { CalendarOverlayZIndex = -1; }

        private void updateOverlay()
        {
            OnPropertyChanged("DaysBookedByCurrentUser");
        }

        private void performTooManyDaysAnimation()
        {
            // it's a monostable kek
            TooManyDaysMonostable = true;
            TooManyDaysMonostable = false;
        }
    }
}
