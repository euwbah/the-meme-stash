using APPD.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPD.ViewModels
{
    public class HomeViewModel : ObservableObject, IPageViewModel
    {
        private MainViewModel parent;

        public HomeViewModel(MainViewModel parent)
        {
            this.parent = parent;
        }
    }
}
