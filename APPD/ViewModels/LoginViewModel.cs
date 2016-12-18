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

        public LoginViewModel(MainViewModel parent)
        {
            this.parent = parent;
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
        #endregion

        private void logIn(PasswordBox passwordBoxControl)
        {
            bool success = UserServices.LogIn(Username, passwordBoxControl.Password);

            if(success)
            {
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
                    LogInErrorDisplay = "Username and password not entered";
                else if (noUsername)
                    LogInErrorDisplay = "Username not entered";
                else if (noPassword)
                    LogInErrorDisplay = "Password not entered";
                else
                    LogInErrorDisplay = "Wrong username and/or password";
                
            }
        }
    }
}
