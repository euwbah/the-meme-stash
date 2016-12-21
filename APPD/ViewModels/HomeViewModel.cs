using APPD.Helpers;
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

namespace APPD.ViewModels
{
    public class HomeViewModel : ObservableObject, IPageViewModel
    {
        private MainViewModel parent;

        private string _usernameDisplayText;
        private string _currentSearchString;
        private ArrayList _displayedItems;

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

        public HomeViewModel(MainViewModel parent)
        {
            this.parent = parent;
            this.DisplayedItems = new ArrayList();
        }

        public void PageOpen()
        {
            updateLocals();
        }

        private void updateLocals()
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
