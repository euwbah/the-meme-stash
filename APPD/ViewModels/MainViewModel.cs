using APPD.Helpers;
using APPD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace APPD.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        //private ICommand _changePageCommand;
        private IPageViewModel _currentPageViewModel;
        private Dictionary<string, IPageViewModel> _pageViewModels;
        
        public ApplicationState State { get; set; }

        public MainViewModel()
        {
            // Add available pages
            PageViewModels.Add("Login", new LoginViewModel(this));
            PageViewModels.Add("Home", new HomeViewModel(this));


            // Set starting page
            CurrentPageViewModel = PageViewModels["Login"];

            // Instantiate global model

            State = new ApplicationState();
        }

        public Dictionary<string, IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new Dictionary<string, IPageViewModel>();

                return _pageViewModels;
            }
        }

        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }

        internal void ChangeViewModel(string pageName)
        {
            CurrentPageViewModel = PageViewModels[pageName];
            CurrentPageViewModel.PageOpen();
        }
    }
}
