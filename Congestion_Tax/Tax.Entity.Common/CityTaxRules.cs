using System;
using System.Collections.Generic;

namespace Tax.Entity.Common
{
    /// <summary>
    /// CityTaxRules object is the list of Tax rule specific to City 
    /// </summary>
    public class CityTaxRules
    {
        public List<TaxRule> CityTaxRuleList { get; set; }
    }

    /// <summary>
    /// TaxRule defines the amount of each Start and End Time Range.
    /// </summary>
    public class TaxRule
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Amount { get; set; }
    }
}
