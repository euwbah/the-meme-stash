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

        public AccountCredential(string socialMediaPlatform, string username, string email, string password)
        {
            if (socialMediaPlatform.Trim() == "")
                throw new ArgumentNullException("SocialMediaPlatform cannot be empty!");
            if (username.Trim() == "" && email.Trim() == "")
                throw new ArgumentNullException("Either username or email must be specified");
            if (password.Trim() == "")
                throw new ArgumentNullException("A password must be specified");

            this.SocialMediaPlatformName = socialMediaPlatform;
            this.Username = username;
            this.Email = email;
            this.Password = password;
        }
    }
}
