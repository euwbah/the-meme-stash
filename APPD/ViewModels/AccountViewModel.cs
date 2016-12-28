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

        #region Fields
        private Account _currentAccount;
        private List<AccountRentalData> _rentalDataList;
        private Visibility _accountCredentialsVisibility;

        private Visibility _ownedLabelVisibility;
        private string _additionalInformation;
        private bool _confirmButtonEnabled;

        private string _actionButtonContent;
        private Brush _actionButtonForeground;
        private Brush _actionButtonBackground;

        private ICommand _actionButtonClick;
        private ICommand _overlayCloseClick;
        private ICommand _backButtonClick;
        private ICommand _confirmButtonClick;

        private int _calendarOverlayZIndex;
        
        // For manually handling the coloring of the calendar
        private List<DateTime> _selectedDates;

        private bool _problemMonostable;
        private bool _tooManyDaysMonostable;
        private bool _notDankEnoughMonostable;

        private int _bookingCost;
        private int _balanceAfterBooking;
        #endregion

        #region Properties
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
        public bool ConfirmButtonEnabled
        {
            get { return _confirmButtonEnabled; }
            set
            {
                if (_confirmButtonEnabled != value)
                {
                    _confirmButtonEnabled = value;
                    OnPropertyChanged("ConfirmButtonEnabled");
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
        public ICommand ConfirmButtonClick
        {
            get
            {
                if (_confirmButtonClick == null)
                {
                    _confirmButtonClick = new RelayCommand(param => this.confirmButtonClicked());
                }

                return _confirmButtonClick;
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
                return ConfigServices.MAX_DAYS_BOOKABLE_PER_ACCOUNT_PER_USER;
            }
        }

        public bool ProblemMonostable
        {
            get { return _problemMonostable; }
            set
            {
                _problemMonostable = value;
                OnPropertyChanged("ProblemMonostable");
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
        public bool NotDankEnoughMonostable
        {
            get { return _notDankEnoughMonostable; }
            set
            {
                _notDankEnoughMonostable = value;
                OnPropertyChanged("NotDankEnoughMonostable");
            }
        }

        public int BookingCost
        {
            get { return _bookingCost; }
            set
            {
                if (value != _bookingCost)
                {
                    _bookingCost = value;
                    OnPropertyChanged("BookingCost");
                }
            }
        }

        public int BalanceAfterBooking
        {
            get { return _balanceAfterBooking; }
            set
            {
                if (value != _balanceAfterBooking)
                {
                    _balanceAfterBooking = value;
                    OnPropertyChanged("BalanceAfterBooking");
                }
            }
        }
        #endregion

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
                AdditionalInformation = "";

                ActionButtonForeground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                ActionButtonBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x99, 0x66, 0x11));

                isActionButtonActivated = false;
            }
            else if (userAlreadyConfirmedBookingDates.Count == ConfigServices.MAX_DAYS_BOOKABLE_PER_ACCOUNT_PER_USER)
            {
                // User has reach max days bookable per account quote

                OwnedLabelVisibility = Visibility.Hidden;
                ActionButtonContent = "VIEW DATES";
                AdditionalInformation = "You currently have this account booked for the maximum of " +
                                        ConfigServices.MAX_DAYS_BOOKABLE_PER_ACCOUNT_PER_USER + 
                                        " days.";

                ActionButtonForeground = new SolidColorBrush(Color.FromArgb(0xBB, 0xFF, 0xFF, 0xFF));
                ActionButtonBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x88, 0x44, 0x44));

                isActionButtonActivated = true;
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
                AdditionalInformation = "";

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
                BalanceAfterBooking = parent.parent.State.CurrentLoggedOnUser.Dankness;

                // This part initializes the calendar

                // Updates the calendar's display with h4x
                calendar.DisplayDate = calendar.DisplayDate.AddYears(1);
                calendar.DisplayDate = calendar.DisplayDate.AddYears(-1);

                calendar.DisplayDateStart = DateTime.Today;
                calendar.DisplayDateEnd = DateTime.Today.AddDays(30);

                // Add event handler
                calendar.SelectedDatesChanged += Calendar_SelectedDatesChanged;

                ConfirmButtonEnabled = false;

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
                    if (CurrentAccount.getListOfBookableDates().Contains(date))
                    {
                        if (DaysBookedByCurrentUser >= ConfigServices.MAX_DAYS_BOOKABLE_PER_ACCOUNT_PER_USER &&
                            BalanceAfterBooking - CurrentAccount.DanknessPerDay >= 0)
                        {
                            performTooManyDaysAnimation();
                        }
                        else if (BalanceAfterBooking - CurrentAccount.DanknessPerDay < 0)
                        {
                            performNotDankEnoughAnimation();
                        }
                        else
                        {
                            SelectedDates.Add(date);
                        }
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

            this.BookingCost = this.SelectedDates.Count * CurrentAccount.DanknessPerDay;
            this.BalanceAfterBooking = parent.parent.State.CurrentLoggedOnUser.Dankness - BookingCost;

            if (SelectedDates.Count == 0)
                ConfirmButtonEnabled = false;
            else
                ConfirmButtonEnabled = true;
        }

        private void overlayCloseClicked()
        {
            hideOverlay();
            this.SelectedDates.RemoveAll(iHadAnAbsoluteShitChristmas => true);
        }
        private void backButtonClicked()
        {
            parent.PageOpen();
            parent.Go(from: Screen.ACCOUNT, to: Screen.HOME);
        }
        private void confirmButtonClicked()
        {
            if (SelectedDates.Count != 0) // Just checking...
            {
                // This will be null if something is wrong
                User transactingUser = parent.parent.State.CurrentLoggedOnUser.PerformRental(SelectedDates, CurrentAccount);
                if (transactingUser != null)
                {
                    this.updateProperties();
                    hideOverlay();
                }
                else
                {
                    MessageBox.Show("There was an error perfoming the booking");
                    this.updateProperties();
                    hideOverlay();
                }
            }
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
            ProblemMonostable = true;
            ProblemMonostable = false;
            TooManyDaysMonostable = true;
            TooManyDaysMonostable = false;
        }

        private void performNotDankEnoughAnimation()
        {
            ProblemMonostable = true;
            ProblemMonostable = false;
            NotDankEnoughMonostable = true;
            NotDankEnoughMonostable = false;
        }
    }
}
