using System;
using System.Collections.Generic;

namespace Tax.Entity.Common
{
    /// <summary>
    /// DayTravel object defines the list of Dates traveled per day.
    /// </summary>
    public class DayTravel
    {
        /// <summary>
        /// This represent the unique day traveled
        /// </summary>
        public DateTime DayId { get; set; }

        /// <summary>
        /// This is hold the list of days traveled on same day
        /// </summary>
        public List<DateTime> DateList { get; set; }
    }
}
