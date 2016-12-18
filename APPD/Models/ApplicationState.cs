using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPD.Models
{
    public class ApplicationState
    {
        #region Current Logged On User
        private User _currentLoggedOnUser;

        public User CurrentLoggedOnUser
        {
            get
            {
                if (_currentLoggedOnUser == null)
                    throw new Exception("Program failure: tried to access ApplicationState.CurrentLoggedOnUser when null");
                else
                    return _currentLoggedOnUser;
            }
            set
            {
                _currentLoggedOnUser = value;
            }
        }
        #endregion
        
        public ApplicationState()
        {

        }
    }
}
