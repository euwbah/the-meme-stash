using APPD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPD.Services
{
    public static class UserServices
    {

        public static List<User> readUsersFromDatabase()
        {
            string json = File.ReadAllText("Users.json");
            User u1 = JsonConvert.DeserializeObject<User>(json);

            var lines = File.ReadAllLines("Users.json");
            foreach (var line in lines)
            {
                yield return JsonConvert.DeserializeObject<User>(json);
            }
        }

        public static User LogIn(string username, string password)
        {
            string json = FileStyleUriParser.ReadAllText("Users.json");
            var playerList = json.DeserializeObject<List<Player>>(json);

            File.WriteAllText("Users.json", json.SerializeObject(playerList));
            return null;
        }
    }
}
