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
                    StreamReader reader = new StreamReader(Application.GetContentStream(new Uri(fileURI)).Stream);
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

        internal static List<Account> PerformSearch(string searchQuery, int from, int to, SearchMethod searchMethod)
        {
            // Filter out non alphanumeric and non whitespace characters
            Regex regex = new Regex(@"[\s\da-zA-Z]");
            searchQuery = new string(searchQuery.Where(chr =>
            {
                MatchCollection matches = regex.Matches(chr.ToString());
                return matches.Count != 0;
            }).ToArray());

            List<string> queryWords = new List<string>(Regex.Split(searchQuery, @"\s+"));

            List<AccountSearchWrapper> searchObjects = new List<AccountSearchWrapper>();

            List<Account> accounts = AccountServices.getAccountsFromDatabase();
            foreach (Account a in accounts)
                searchObjects.Add(new AccountSearchWrapper(a));

            while (queryWords.Count != 0)
            {
                test(queryWords, searchObjects, searchMethod);
                queryWords.RemoveAt(0);
            }

            double totalSearchScore = 0;

            searchObjects.ForEach(searchObject => totalSearchScore += searchObject.SearchScore);

            double averageSearchScore = totalSearchScore / searchObjects.Count;

            // Return accounts that are in the top 75 percentile
            List<AccountSearchWrapper> filteredSearchObjects =
                searchObjects.Where(searchObject => searchObject.SearchScore > averageSearchScore / 2.0).ToList();

            List<Account> returnable = (
                from s in filteredSearchObjects
                orderby s.SearchScore descending
                select s.Item
                ).ToList();

            //NOTE: from and to are 1-indexed!
            int count;

            if (from < 1) from = 1;
            else if (from > returnable.Count) {
                return new List<Account>(); //Nothing
            }

            // range is inclusive, count has no index (just an cardinal)
            count = to - from + 1;
            if (count < 0) count = 0;
            if (count + from > returnable.Count + 1) count = returnable.Count - from + 1;

            return returnable.GetRange(from - 1, count);
        }

        private static void test(List<string> queryWords, List<AccountSearchWrapper> searchObjects, SearchMethod method)
        {
            string queryUniformWhitespaceString = string.Join(" ", queryWords).ToLower();

            foreach(AccountSearchWrapper searchObject in searchObjects)
            {
                searchObject.scoreAgainst(queryUniformWhitespaceString, method);
            }
        }

        protected abstract class SearchWrapper<T>
        {
            public T Item { get; private set; }
            public double SearchScore { get; set; }

            public SearchWrapper(T Item)
            {
                this.Item = Item;
                SearchScore = 0d;
            }

            public abstract void scoreAgainst(string queryString, SearchMethod searchMethod);
        }

        protected class AccountSearchWrapper : SearchWrapper<Account>
        {
            public AccountSearchWrapper(Account Item) : base(Item) { }

            public override void scoreAgainst(string queryString, SearchMethod searchMethod)
            {
                Regex nonAlphanumericOrWhitespace = new Regex(@"[^\s\da-zA-Z]");

                List<string> titleWords = Regex.Split(nonAlphanumericOrWhitespace.Replace(Item.Name, ""), @"\s+")
                                        .Select(asdf => asdf.ToLower()).ToList();

                List<string> tags = Item.Tags;

                double score = 0;

                foreach(string tag in tags.Select(tag => tag.ToLower()))
                {
                    for(int i = 0; i < queryString.Length && i < tag.Length; i++)
                    {
                        if(tag[i] == queryString[i])
                        {
                            score += 1.0 / tag.Length;
                        }
                        else
                        {
                            break;
                        }
                    }
                }


                for (int i = 0; i < titleWords.Count; i++)
                {
                    string currWord = titleWords[i];

                    for (int idx = 0; idx < currWord.Length && idx < queryString.Length; idx++)
                    {
                        if (currWord[idx] == queryString[idx])
                        {
                            score += 2.0 / currWord.Length;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                this.SearchScore += score;
            }
        }

        internal enum SearchMethod
        {
            RELEVANCE
        }
    }
}
