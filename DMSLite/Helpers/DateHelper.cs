using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMSLite.Helpers
{
    using DateRange = Tuple<DateTime, DateTime>;
    public static class DateHelper
    {
        //refactored into a helper class
        public static DateRange DateFromRange(string date, string datePeriod, string dateComparator)
        {
            if (!String.IsNullOrWhiteSpace(date))
            {
                DateTime dateValue = ConvertDate(date);
                return Tuple.Create(StartOfDay(dateValue), EndOfDay(dateValue));
            }
            if (!String.IsNullOrWhiteSpace(datePeriod))
            {
                DateTime startOfDay = DateTime.ParseExact(datePeriod.Split('/')[0], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                DateTime endOfDay = DateTime.ParseExact(datePeriod.Split('/')[1], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                if ((startOfDay.Year > DateTime.Today.Year) && ((endOfDay - startOfDay).TotalDays >= 364 && !dateComparator.Equals("<")))
                {
                    //this is a full year, do not handle it unless the range is before
                    return null;
                }
                return Tuple.Create(
                    startOfDay,
                    endOfDay
                    );
            }
            return null;
        }

        public static DateTime ConvertDate(string date)
        {
            //adjusts date to match current date (for future dates)
            DateTime convertedDate;
            if (date.Length == 4)
                convertedDate = DateTime.ParseExact((date + "-01-01"), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            else
                convertedDate = DateTime.ParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            //If date in the future, send it into the past (only year for year basis)
            if (convertedDate.CompareTo(DateTime.Today) > 0)
                convertedDate = DateTime.Today.Year - convertedDate.Year == 0 ? convertedDate.AddYears(-1) : convertedDate.AddYears(DateTime.Today.Year - convertedDate.Year);

            return convertedDate;
        }

        public static DateTime StartOfDay(DateTime dateTime)
        {
            return dateTime.Date;
        }

        public static DateTime EndOfDay(DateTime dateTime)
        {
            return StartOfDay(dateTime).AddDays(1).AddTicks(-1);
        }

    }
}