using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace APPD.Views
{
    public partial class LoginView
    {
        void passwordBoxFocused(object sender, EventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            passwordBox.Background.Opacity = 0;
        }
        void passwordBoxLostFocus(object sender, EventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox.Password.Length == 0)
                passwordBox.Background.Opacity = 1;
        }
    }
}
