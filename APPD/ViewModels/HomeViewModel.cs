using APPD.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace APPD.ViewModels
{
    public class HomeViewModel : ObservableObject, IPageViewModel
    {
        private MainViewModel parent;

        private string _usernameDisplayText;
        private string _currentSearchString;

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
            get { return _currentSearchString; }
            set
            {
                if (value != _currentSearchString)
                {
                    _currentSearchString = value;
                    OnPropertyChanged("CurrentSearchString");
                }
            }
        }

        public HomeViewModel(MainViewModel parent)
        {
            this.parent = parent;
        }

        public void PageOpen()
        {
            updateLocals();
        }

        private void updateLocals()
        {
            this.UsernameDisplayText = parent.State.CurrentLoggedOnUser.Username;
        }
    }
}
