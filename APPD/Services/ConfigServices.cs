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
    internal static class ConfigServices
    {
        #region MAX DAYS BOOKABLE IN ADVANCE
        private static short? _maxDaysBookableInAdvance;
        internal static short MAX_DAYS_BOOKABLE_IN_ADVANCE
        {
            get
            {
                if (_maxDaysBookableInAdvance == null)
                    ReadState();

                return _maxDaysBookableInAdvance.GetValueOrDefault();
            }
        }
        #endregion

        #region MAX DAYS BOOKABLE PER ACCOUNT PER USER
        private static short? _maxDaysBookablePerAccountPerUser;
        internal static short MAX_DAYS_BOOKABLE_PER_ACCOUNT_PER_USER
        {
            get
            {
                if (_maxDaysBookablePerAccountPerUser == null)
                    ReadState();

                return _maxDaysBookablePerAccountPerUser.GetValueOrDefault();
            }
        }
        #endregion

        internal static int UserIDCounter
        {
            get
            {
                string fileURI = "pack://application:,,,/Assets/SimulatedServer/Database/Config.json";
                StreamReader reader = new StreamReader(Application.GetResourceStream(new Uri(fileURI)).Stream);
                string configJsonFileContents = reader.ReadToEnd();

                var definition = new { UserIDCounter = 0 };
                var aaaaa = JsonConvert.DeserializeAnonymousType(configJsonFileContents, definition);

                return aaaaa.UserIDCounter;
            }
        }

        internal static int AccountIDCounter
        {
            get
            {
                string fileURI = "pack://application:,,,/Assets/SimulatedServer/Database/Config.json";
                StreamReader reader = new StreamReader(Application.GetResourceStream(new Uri(fileURI)).Stream);
                string configJsonFileContents = reader.ReadToEnd();

                var definition = new { AccountIDCounter = 0 };
                var aaaaa = JsonConvert.DeserializeAnonymousType(configJsonFileContents, definition);

                return aaaaa.AccountIDCounter;
            }
        }

        internal static void ReadState()
        {
            string fileURI = "pack://application:,,,/Assets/SimulatedServer/Database/Config.json";

            StreamReader reader = new StreamReader(Application.GetResourceStream(new Uri(fileURI)).Stream);

            string configJsonFileContents = reader.ReadToEnd();

            var definition = new
            {
                UserIDCounter = 0,
                AccountIDCounter = 0,
                MaxDaysBookableInAdvance = 0,
                MaxDaysBookablePerAccountPerUser = 0
            };

            var config = JsonConvert.DeserializeAnonymousType(configJsonFileContents, definition);

            _maxDaysBookableInAdvance = (short?) config.MaxDaysBookableInAdvance;
            _maxDaysBookablePerAccountPerUser = (short?)config.MaxDaysBookablePerAccountPerUser;
        }
    }
}
