using APPD.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace APPD.Services
{
    internal static class SearchServices
    {
        private static List<string> _trendingTags;
        internal static List<string> TrendingTags
        {
            get
            {
                if (_trendingTags == null)
                {
                    string fileURI = "pack://application:,,,/Assets/SimulatedServer/Database/State.json";
                    StreamReader reader = new StreamReader(Application.GetResourceStream(new Uri(fileURI)).Stream);
                    string stateJsonFileContents = reader.ReadToEnd();

                    var definition = new
                    {
                        TrendingTags = new List<string>()
                    };

                    var aaaaa = JsonConvert.DeserializeAnonymousType(stateJsonFileContents, definition);

                    _trendingTags = aaaaa.TrendingTags;
                }

                return _trendingTags;
            }
        }
        
        //internal static List<Account> PerformSearch(string searchQuery, int from, int to)
        //{
        //    // Filter out non alphanumeric and non whitespace characters
        //    Regex regex = new Regex(@"[\s\da-zA-Z]");
        //    searchQuery = new string(searchQuery.Where(chr =>
        //    {
        //        MatchCollection matches = regex.Matches(chr.ToString());
        //        return matches.Count != 0;
        //    }).ToArray());

        //    List<string> queryWords = new List<string>(Regex.Split(searchQuery, @"\s+"));

        //    List<AccountSearchWrapper> searchObjects = new List<AccountSearchWrapper>();

        //    List<Account> accounts = AccountServices.getAccountsFromDatabase();
        //    foreach(Account a in accounts)
        //    {
        //        accoun
        //    }
        //}

        protected abstract class SearchWrapper<T>
        {
            public T Item { get; private set; }
            public int SearchScore { get; set; }

            public SearchWrapper(T Item)
            {
                this.Item = Item;
            }

            public abstract void scoreAgainst(string queryString);
        }

        protected class AccountSearchWrapper : SearchWrapper<Account>
        {
            public AccountSearchWrapper(Account Item) : base(Item)
            {

            }
            public override void scoreAgainst(string queryString)
            {

            }
        }
    }
}
