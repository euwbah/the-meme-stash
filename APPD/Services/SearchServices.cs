using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    }
}
