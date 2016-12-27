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
        private ICommand _searchBarLostFocus;
        private ICommand _searchBarGotFocus;
        private bool searchBarFocused;

        private bool _changeScreenAnimationMonostable;
        private double _xTransformFromValue;



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
                if (value != _currentSearchString)
                {
                    _currentSearchString = value;
                    OnPropertyChanged("CurrentSearchString");
                    this.performDisplayListViewUpdate();
                }
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
                        parent.State.CurrentLoggedOnUser = null;
                    });
                }

                return _logoutCommand;
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

        private void performDisplayListViewUpdate()
        {
           if (this.CurrentSearchString.Trim().Length == 0)
            {
                DisplayedItems = new ArrayList();
                DisplayedItems.Add(new TextWrapper("FEATURED"));
                DisplayedItems.AddRange(AccountServices.getFeaturedAccounts());

                DisplayedItems.Add(new TextWrapper("NEW"));
                DisplayedItems.AddRange(AccountServices.getNewAccounts());
            }
            else
            {
                List<Account> searchedAccounts =
                    SearchServices.PerformSearch(CurrentSearchString, 0, 5, SearchServices.SearchMethod.RELEVANCE);

                DisplayedItems = new ArrayList();
                DisplayedItems.AddRange(searchedAccounts);

                //DisplayedItems.RemoveRange(0, DisplayedItems.Count);
                //DisplayedItems.AddRange(searchedAccounts);
            }
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

        internal void Go(Screen from, Screen to)
        {
            this.B = from.ConvertToXTransformValue();
            var finalPosition = to.ConvertToXTransformValue();
            this.A = finalPosition - this.B;
            this.triggerAnimation();
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

    public class TextWrapper : ObservableObject
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

        public TextWrapper(string text)
        {
            this.Text = text;
        }
    }
}
