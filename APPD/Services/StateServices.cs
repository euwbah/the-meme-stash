using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPD.Services
{
    internal static class StateServices
    {
        #region MAX DAYS BOOKABLE IN ADVANCE
        private static short? _maxDaysBookableInAdvance;
        internal static short MAX_DAYS_BOOKABLE_IN_ADVANCE
        {
            get
            {
                if (_maxDaysBookableInAdvance == null)
                    _maxDaysBookableInAdvance = getMaxNumberOfBookableDaysFromToday();

                return _maxDaysBookableInAdvance.GetValueOrDefault();
            }
        }
        private static short getMaxNumberOfBookableDaysFromToday()
        {
            //TODO: Actually read this from the server
            return 30;
        }
        #endregion

        #region MAX DAYS BOOKABLE PER ACCOUNT PER USER
        private static short? _maxDaysBookablePerAccountPerUser;
        internal static short MAX_DAYS_BOOKABLE_PER_ACCOUNT_PER_USER
        {
            get
            {
                if (_maxDaysBookablePerAccountPerUser == null)
                    _maxDaysBookablePerAccountPerUser = getMaxDaysBookablePerAccountPerUser();

                return _maxDaysBookablePerAccountPerUser.GetValueOrDefault();
            }
        }
        private static short getMaxDaysBookablePerAccountPerUser()
        {
            //TODO: Actually read this from the server
            return 4;
        }
        #endregion

    }
}
