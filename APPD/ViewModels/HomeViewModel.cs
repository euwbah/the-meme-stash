using APPD.Helpers;
using APPD.Models;
using APPD.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace APPD.ViewModels
{
    public class HomeViewModel : ObservableObject, IPageViewModel
    {
        public MainViewModel parent { get; private set; }

        private string _usernameDisplayText;
        private string _usernameDanknessDisplayText;
        private string _currentSearchString;
        private ArrayList _displayedItems;
        private List<string> _trendingTags;

        private ICommand _accountCommand;
        private ICommand _logoutCommand;
        private ICommand _displayCommand;
        private ICommand _trendingTagCommand;
        private ICommand _searchBarLostFocus;
        private ICommand _searchBarGotFocus;
        private bool searchBarFocused;

        private bool _changeScreenAnimationMonostable;
        private double _xTransformFromValue;

        private Display _currentDisplay;

        public string UsernameDisplayText
        {
            get { return _usernameDisplayText; }
            set
            {
                if (value != _usernameDisplayText)
                {
                    _usernameDisplayText = value;
                    OnPropertyChanged("UsernameDisplayText");
                }
            }
        }
        public string UsernameDanknessDisplayText
        {
            get { return _usernameDanknessDisplayText; }
            set
            {
                if (value != _usernameDanknessDisplayText)
                {
                    _usernameDanknessDisplayText = value;
                    OnPropertyChanged("UsernameDanknessDisplayText");
                }
            }
        }
        public string CurrentSearchString
        {
            get { return _currentSearchString ?? ""; }
            set
            {
                _currentSearchString = value;
                OnPropertyChanged("CurrentSearchString");
                this.performDisplayListViewUpdate();
            }
        }
        public ArrayList DisplayedItems
        {
            get
            {
                return _displayedItems;
            }
            set
            {
                if (_displayedItems != value)
                {
                    _displayedItems = value;
                    OnPropertyChanged("DisplayedItems");
                }
            }
        }
        public List<string> TrendingTags
        {
            get { return _trendingTags; }
            set
            {
                if (value != _trendingTags)
                {
                    _trendingTags = value;
                    OnPropertyChanged("TrendingTags");
                }
            }
        }

        public ICommand AccountCommand
        {
            get
            {
                if (_accountCommand == null)
                {
                    _accountCommand = new RelayCommand(
                        account =>
                        {
                            AccountViewModel.CurrentAccount = (Account)account;
                            Go(from: Screen.HOME, to: Screen.ACCOUNT);
                        }
                    );
                }

                return _accountCommand;
            }
        }
        public ICommand LogoutCommand
        {
            get
            {
                if (_logoutCommand == null)
                {
                    _logoutCommand = new RelayCommand(param =>
                    {
                        parent.ChangeViewModel("Login");
                    });
                }

                return _logoutCommand;
            }
        }
        public ICommand DisplayCommand
        {
            get
            {
                if (_displayCommand == null)
                    _displayCommand = new RelayCommand(text => this.changeDisplayClicked(((string)text).asDisplay()));

                return _displayCommand;
            }
        }
        public ICommand TrendingTagCommand
        {
            get
            {
                if (_trendingTagCommand == null)
                    _trendingTagCommand = new RelayCommand(tagStr => this.trendingTagClicked((string)tagStr));

                return _trendingTagCommand;
            }
        }
        public ICommand SearchBarLostFocus
        {
            get
            {
                if (_searchBarLostFocus == null)
                {
                    _searchBarLostFocus = new RelayCommand(param => this.searchLostFocus());
                }

                return _searchBarLostFocus;
            }
        }
        public ICommand SearchBarGotFocus
        {
            get
            {
                if (_searchBarGotFocus == null)
                {
                    _searchBarGotFocus = new RelayCommand(param => this.searchGotFocus());
                }

                return _searchBarGotFocus;
            }
        }

        private double _a, _b;
        public double A
        {
            get
            {
                return _a;
            }
            set
            {
                _a = value;
                OnPropertyChanged("A");
            }
        }
        public double B
        {
            get { return _b; }
            set
            {
                _b = value;
                OnPropertyChanged("B");
            }
        }

        public bool ChangeScreenAnimationMonostable
        {
            get { return _changeScreenAnimationMonostable; }
            set
            {
                if (value != _changeScreenAnimationMonostable)
                {
                    _changeScreenAnimationMonostable = value;
                    OnPropertyChanged("ChangeScreenAnimationMonostable");
                }
            }
        }
        public double XTransformFromValue
        {
            get { return _xTransformFromValue; }
            set
            {
                if (value != _xTransformFromValue)
                {
                    _xTransformFromValue = value;
                    OnPropertyChanged("XTransformFromValue");
                }
            }
        }
        
        private Display currentDisplay
        {
            get { return _currentDisplay; }
            set
            {
                _currentDisplay = value;
                OnPropertyChanged("ExploreForeground");
                OnPropertyChanged("ExploreBackground");
                OnPropertyChanged("OwnedForeground");
                OnPropertyChanged("OwnedBackground");
                OnPropertyChanged("RentedForeground");
                OnPropertyChanged("RentedBackground");
            }
        }

        public SolidColorBrush ExploreForeground
        {
            get
            {
                return new SolidColorBrush(currentDisplay == Display.EXPLORE ?
                    Color.FromArgb(0xff, 0x00, 0x00, 0x00) : Color.FromArgb(0xff, 0xff, 0xff, 0xff));
            }
        }
        public SolidColorBrush ExploreBackground
        {
            get
            {
                return new SolidColorBrush(currentDisplay == Display.EXPLORE ?
                    Color.FromArgb(0xff, 0xff, 0xff, 0xff) : Color.FromArgb(0xff, 0x00, 0x00, 0x00));
            }
        }
        public SolidColorBrush OwnedForeground
        {
            get
            {
                return new SolidColorBrush(currentDisplay == Display.OWNED ?
                    Color.FromArgb(0xff, 0x00, 0x00, 0x00) : Color.FromArgb(0xff, 0xff, 0xff, 0xff));
            }
        }
        public SolidColorBrush OwnedBackground
        {
            get
            {
                return new SolidColorBrush(currentDisplay == Display.OWNED ?
                    Color.FromArgb(0xff, 0xff, 0xff, 0xff) : Color.FromArgb(0xff, 0x00, 0x00, 0x00));
            }
        }
        public SolidColorBrush RentedForeground
        {
            get
            {
                return new SolidColorBrush(currentDisplay == Display.RENTED ?
                    Color.FromArgb(0xff, 0x00, 0x00, 0x00) : Color.FromArgb(0xff, 0xff, 0xff, 0xff));
            }
        }
        public SolidColorBrush RentedBackground
        {
            get
            {
                return new SolidColorBrush(currentDisplay == Display.RENTED ?
                    Color.FromArgb(0xff, 0xff, 0xff, 0xff) : Color.FromArgb(0xff, 0x00, 0x00, 0x00));
            }
        }

        public AccountViewModel AccountViewModel { get; set; }

        public HomeViewModel(MainViewModel parent)
        {
            this.parent = parent;
            this.DisplayedItems = new ArrayList();
            this.XTransformFromValue = 0;
            this.Go(from: Screen.HOME, to: Screen.HOME);
            this.AccountViewModel = new AccountViewModel(this);
        }

        public void PageOpen()
        {
            updateProperties();
        }

        private void updateProperties()
        {
            this.TrendingTags = SearchServices.TrendingTags;

            this.UsernameDisplayText = parent.State.CurrentLoggedOnUser.Username;
            this.UsernameDanknessDisplayText = parent.State.CurrentLoggedOnUser.Dankness.ToString();
            parent.State.CurrentLoggedOnUser.OnDanknessUpdated += updateDanknessDisplay;

            this.performDisplayListViewUpdate();
        }
        private void updateProperties(bool resetDisplay)
        {
            if (resetDisplay)
                currentDisplay = Display.EXPLORE;
            updateProperties();
        }

        private void performDisplayListViewUpdate()
        {
            DisplayedItems = new ArrayList();
            if (this.CurrentSearchString.Trim().Length == 0)
            {

                List<Account> featuredAccounts = AccountServices.getFeaturedAccounts();
                List<Account> ownedAccounts = AccountServices.getAccountsOwnedBy(parent.State.CurrentLoggedOnUser);

                if (currentDisplay == Display.EXPLORE)
                {

                    DisplayedItems.Add(new Header("FEATURED"));
                    DisplayedItems.AddRange(featuredAccounts);

                    DisplayedItems.Add(new Header("NEW"));
                    int nthCounter = 0;
                    for (int numberOfNewAccountsDisplayed = 0; numberOfNewAccountsDisplayed < 3;)
                    {
                        Account a = AccountServices.getNthNewestAccount(nthCounter);
                        if (a == null) break;

                        nthCounter++;

                        if ((featuredAccounts.Where(acc => a.ID == acc.ID).Count() == 0) &&
                            (ownedAccounts.Where(acc => a.ID == acc.ID).Count() == 0))
                        {
                            DisplayedItems.Add(a);
                            numberOfNewAccountsDisplayed++;
                        }
                    }
                }
                else if (currentDisplay == Display.OWNED)
                {
                    DisplayedItems.Add(new Header("OWNED"));
                    if (ownedAccounts.Count != 0)
                    {
                        DisplayedItems.AddRange(ownedAccounts);
                    }
                    else
                        DisplayedItems.Add(new Info("You haven't authored any accounts yet! Lease an account now and get more Đankness!"));
                }
                else if (currentDisplay == Display.RENTED)
                {
                    List<Account> currentlyInPossession =
                        AccountServices.getAccountsRentedBy(parent.State.CurrentLoggedOnUser, true);
                    List<Account> notCurrentlyInPossession =
                        AccountServices.getAccountsRentedBy(parent.State.CurrentLoggedOnUser, false);

                    if (currentlyInPossession.Count != 0 || notCurrentlyInPossession.Count != 0)
                    {
                        DisplayedItems.Add(new Header("ACTIVE"));
                        if (currentlyInPossession.Count != 0)
                            DisplayedItems.AddRange(currentlyInPossession);
                        else
                            DisplayedItems.Add(new Info("You don't have any accounts currently in possession"));

                        if (notCurrentlyInPossession.Count != 0)
                        {
                            DisplayedItems.Add(new Header("BOOKED"));
                            DisplayedItems.AddRange(notCurrentlyInPossession);
                        }
                    }
                    else
                    {
                        DisplayedItems.Add(new Header("RENTED"));
                        DisplayedItems.Add(new Info("You have no booked accounts"));
                    }
                }
            }
            else
            {
                List<Account> searchedAccounts =
                    SearchServices.PerformSearch(CurrentSearchString, 0, 5, SearchServices.SearchMethod.RELEVANCE);

                DisplayedItems.AddRange(searchedAccounts);
            }

            // Add empty text to update and add space at the bottom
            DisplayedItems.Add(new Info(""));
        }

        private void triggerAnimation()
        {
            ChangeScreenAnimationMonostable = true;
            ChangeScreenAnimationMonostable = false;
        }

        private void updateDanknessDisplay(User sender)
        {
            this.UsernameDanknessDisplayText = sender.Dankness.ToString();
        }

        private void searchLostFocus()
        {
            searchBarFocused = false;
            this.performDisplayListViewUpdate();
        }
        private void searchGotFocus()
        {
            searchBarFocused = true;
            this.performDisplayListViewUpdate();
        }
        private void changeDisplayClicked(Display display)
        {
            currentDisplay = display;
            CurrentSearchString = "";
            performDisplayListViewUpdate();
        }
        private void trendingTagClicked(string tagStr)
        {
            currentDisplay = Display.EXPLORE;
            CurrentSearchString = tagStr;
        }

        internal void Go(Screen from, Screen to)
        {
            this.B = from.ConvertToXTransformValue();
            var finalPosition = to.ConvertToXTransformValue();
            this.A = finalPosition - this.B;
            this.triggerAnimation();
        }

    }

    internal enum Display
    {
        EXPLORE, OWNED, RENTED
    }


    internal static class DisplayEnumExtensions
    {
        internal static Display asDisplay(this string str)
        {
            switch (str.ToLower().Trim())
            {
                case "explore":
                    return Display.EXPLORE;
                case "owned":
                    return Display.OWNED;
                case "rented":
                    return Display.RENTED;
                default:
                    return Display.EXPLORE;
            }
        }
    }

    internal enum Screen
    {
        HOME, ACCOUNT
    }

    internal static class ScreenEnumExtensions
    {
        internal static double ConvertToXTransformValue(this Screen screen)
        {
            switch(screen)
            {
                case Screen.HOME:
                    return 0;
                case Screen.ACCOUNT:
                    return -1200;
                default:
                    return 0;
            }
        }
    }

    public class Header : ObservableObject
    {
        private string _text;
        public string Text
        {
            get
            {
                return _text ?? "";
            }
            set
            {
                if (value != _text)
                {
                    _text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        public Header(string text)
        {
            this.Text = text;
        }
    }
    public class Info : ObservableObject
    {
        private string _text;
        public string Text
        {
            get
            {
                return _text ?? "";
            }
            set
            {
                if (value != _text)
                {
                    _text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        public Info(string text)
        {
            this.Text = text;
        }
    }
}
