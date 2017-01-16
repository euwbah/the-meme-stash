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
using System.Windows.Input;

namespace APPD.ViewModels
{
    public class LoginViewModel : ObservableObject, IPageViewModel
    {
        private MainViewModel parent;

        private string _username;
        private string _logInErrorDisplay;

        private ICommand _logInCommand;
        private ICommand _signUpCommand;

        private Visibility _userCreatedMessageVisibility;

        public LoginViewModel(MainViewModel parent)
        {
            this.parent = parent;
            UserCreatedMessageVisibility = Visibility.Collapsed;
        }

        public void PageOpen()
        {
            UserCreatedMessageVisibility = Visibility.Collapsed;
            Username = "";
        }

        #region Properties
        public string Username
        {
            get { return this._username ?? ""; }
            set
            {
                if(value != this._username)
                {
                    this._username = value;
                    OnPropertyChanged("Username");
                }
            }
        }
        public string LogInErrorDisplay
        {
            get
            {
                return _logInErrorDisplay ?? "";
            }
            set
            {
                this._logInErrorDisplay = value;
                OnPropertyChanged("LogInErrorDisplay");
            }
        }

        public ICommand LogInCommand
        {
            get
            {
                if (_logInCommand == null)
                {
                    _logInCommand = new RelayCommand(
                        passwordBoxControl => logIn((PasswordBox)passwordBoxControl)
                    );
                }

                return _logInCommand;
            }
        }
        public ICommand SignUpCommand
        {
            get
            {
                if (_signUpCommand == null)
                    _signUpCommand = new RelayCommand(p => parent.ChangeViewModel("Sign Up"));

                return _signUpCommand;
            }
        }

        public Visibility UserCreatedMessageVisibility
        {
            get { return _userCreatedMessageVisibility; }
            set { _userCreatedMessageVisibility = value; OnPropertyChanged("UserCreatedMessageVisibility"); }
        }
        #endregion


        private void logIn(PasswordBox passwordBoxControl)
        {
            User loggedInUser = UserServices.LogIn(Username, passwordBoxControl.Password);

            if(loggedInUser != null)
            {
                parent.State.CurrentLoggedOnUser = loggedInUser;
                parent.ChangeViewModel("Home");
            }
            else
            {
                bool noUsername = false, noPassword = false;
                if (Username.Trim().Length == 0)
                    noUsername = true;
                if (passwordBoxControl.Password.Length == 0)
                    noPassword = true;

                if (noUsername && noPassword)
                    LogInErrorDisplay = "Enter a username and password!";
                else if (noUsername)
                    LogInErrorDisplay = "Enter a username!";
                else if (noPassword)
                    LogInErrorDisplay = "Enter a password!";
                else
                    LogInErrorDisplay = "Wrong username / password!";
                
            }
        }
    }
}
