using APPD.Helpers;
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
    public class SignUpViewModel : ObservableObject, IPageViewModel
    {
        private MainViewModel parent;

        private string _errorDisplay;
        private string _username;

        private ICommand _registerCommand;
        private ICommand _backToSignInCommand;

        public string Username
        {
            get { return _username ?? ""; }
            set { _username = value; OnPropertyChanged("Username"); }
        }
        public string ErrorDisplay
        {
            get { return _errorDisplay ?? ""; }
            set { _errorDisplay = value; OnPropertyChanged("ErrorDisplay"); }
        }

        public ICommand RegisterCommand
        {
            get
            {
                if (_registerCommand == null)
                    _registerCommand = new RelayCommand(parms => performRegister(parms));

                return _registerCommand;
            }
        }
        public ICommand BackToSignInCommand
        {
            get
            {
                if (_backToSignInCommand == null)
                    _backToSignInCommand = new RelayCommand(parms => parent.ChangeViewModel("Login"));

                return _backToSignInCommand;
            }
        }

        public SignUpViewModel(MainViewModel parent)
        {
            this.parent = parent;
        }

        public void PageOpen()
        {
            ErrorDisplay = "";
            Username = "";
        }

        private void performRegister(object twoPasswordBoxes)
        {
            PasswordBox[] pwdBoxes = (PasswordBox[])twoPasswordBoxes;
            
            if(Username.Trim().Length == 0)
            {
                ErrorDisplay = "Username can't be blank!";
                return;
            }

            if(pwdBoxes[0].Password.Length == 0)
            {
                ErrorDisplay = "Please enter a password!";
                return;
            }

            if(pwdBoxes[0].Password.Length < 6)
            {
                ErrorDisplay = "Password must be at least 6 characters long!";
                return;
            }

            if (pwdBoxes[0].Password != pwdBoxes[1].Password)
            {
                ErrorDisplay = "Passwords didn't match";
                return;
            }

            // If no errors, create new account and bring user back to LoginView, with an 'Account Created' label

            // TODO: Add user creation function

            // Back to LoginView
            parent.ChangeViewModel("Login");
            ((LoginViewModel)parent.PageViewModels["Login"]).UserCreatedMessageVisibility = Visibility.Visible;
        }
    }
}
