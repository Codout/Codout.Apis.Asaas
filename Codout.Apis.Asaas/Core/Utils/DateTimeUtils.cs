using System;

namespace Codout.Apis.Asaas.Core.Utils
{
    internal static class DateTimeUtils
    {

        public static DateTime? Parse(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return null;
            }

            return DateTime.Parse(date);
        }

    }
}