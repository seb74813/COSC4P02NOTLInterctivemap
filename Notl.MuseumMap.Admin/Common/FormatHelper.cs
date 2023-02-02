using System;

namespace Notl.MuseumMap.Admin.Common
{ 
    /// <summary>
    /// Utility method for formatting data.
    /// </summary>
    public class FormatHelper
    {
        /// <summary>
        /// Helper method to turn a distance into km.
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static string Distance(double distance)
        {
            if(distance < 1000)
            {
                return $"{distance:0} meters.";
            }
            else
            {
                return $"{(distance / 1000):0.0} km.";
            }
        }

        /// <summary>
        /// Helper method to turn a date time into an time-ago (age) expression (ex: 'now' or '2 minutes ago')
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string TimeAgo(DateTime dt)
        {
            dt = dt.ToLocalTime();

            if (dt > DateTime.Now)
                return "sometime from now";
            TimeSpan span = DateTime.Now - dt;

            if (span.Days > 365)
            {
                int years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return String.Format("{0} {1} ago", years, years == 1 ? "year" : "years");
            }

            if (span.Days > 30)
            {
                int months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return String.Format("{0} {1} ago", months, months == 1 ? "month" : "months");
            }

            if (span.Days > 0)
                return String.Format("{0} {1} ago", span.Days, span.Days == 1 ? "day" : "days");

            if (span.Hours > 0)
                return String.Format("{0} {1} ago", span.Hours, span.Hours == 1 ? "hour" : "hours");

            if (span.Minutes > 0)
                return String.Format("{0} {1} ago", span.Minutes, span.Minutes == 1 ? "minute" : "minutes");

            if (span.Seconds > 5)
                return String.Format("{0} seconds ago", span.Seconds);

            if (span.Seconds <= 5)
                return "just now";

            return string.Empty;
        }

    }
}
