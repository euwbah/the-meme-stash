﻿using APPD.Helpers;
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
        private string _currentSearchString;
        private ArrayList _displayedItems;

        private Screens _currentScreen;
        private string _currentScreenString;

        private ICommand _accountCommand;
        

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
        public Screens CurrentScreen
        {
            get { return _currentScreen; }
            set
            {
                if (value != _currentScreen)
                {
                    _currentScreen = value;
                    CurrentScreenString = _currentScreen.ToScreenString();
                }
            }
        }
        public string CurrentScreenString
        {
            get { return _currentScreenString; }
            set
            {
                _currentScreenString = value;
                OnPropertyChanged("CurrentScreenString");
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
                            GoToAccount((Account)account);
                        }
                    );
                }

                return _accountCommand;
            }
        }

        public AccountViewModel AccountViewModel { get; set; }


        public HomeViewModel(MainViewModel parent)
        {
            this.parent = parent;
            this.DisplayedItems = new ArrayList();
            this.CurrentScreen = Screens.HOME;
            this.AccountViewModel = new AccountViewModel(this);
        }

        public void PageOpen()
        {
            updateProperties();
        }

        private void updateProperties()
        {
            this.UsernameDisplayText = parent.State.CurrentLoggedOnUser.Username;

            this.performDisplayListViewUpdate();
        }

        private void performDisplayListViewUpdate()
        {
            if (this.CurrentSearchString.Trim().Length == 0)
            {
                DisplayedItems = new ArrayList
                {
                    new TextWrapper("FEATURED")
                };
                DisplayedItems.AddRange(AccountServices.getFeaturedAccounts());

                DisplayedItems.Add(new TextWrapper("NEW"));
                DisplayedItems.AddRange(AccountServices.getNewAccounts());
            }
        }

        private void GoToAccount(Account account)
        {
            AccountViewModel.CurrentAccount = account;
            CurrentScreen = Screens.ACCOUNT;
        }
    }

    public enum Screens
    {
        HOME, ACCOUNT
    }

    public static class ViewsEnumExtensions
    {
        public static string ToScreenString(this Screens v)
        {
            switch(v)
            {
                case Screens.ACCOUNT:
                    return "Account";
                case Screens.HOME:
                    return "Home";
            }

            return "";
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
