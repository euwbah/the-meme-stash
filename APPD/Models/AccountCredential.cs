using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPD.Models
{
    public class AccountCredential
    {
        public string SocialMediaPlatformName { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }

        public AccountCredential(string SocialMediaPlatform, string Username, string Email, string Password)
        {
            if (SocialMediaPlatform.Trim() == "")
                throw new ArgumentNullException("SocialMediaPlatform cannot be empty!");
            if (Username.Trim() == "" && Email.Trim() == "")
                throw new ArgumentNullException("Either username or email must be specified");
            if (Password.Trim() == "")
                throw new ArgumentNullException("A password must be specified");

            this.SocialMediaPlatformName = SocialMediaPlatform.ToUpper();
            this.Username = Username;
            this.Email = Email;
            this.Password = Password;
        }
    }
}
