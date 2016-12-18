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

        private ICommand _logInCommand;

        public LoginViewModel(MainViewModel parent)
        {
            this.parent = parent;
        }

        #region Properties
        public string Username
        {
            get { return this._username; }
            set
            {
                if(value != this._username)
                {
                    this._username = value;
                    OnPropertyChanged("Username");
                }
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
        }
    }
}
