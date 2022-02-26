using System;
using System.Collections.Generic;
using System.Linq;
using Tax.Calculator.Helper.Interface;
using Tax.Entity.Common;
using Tax.Rules.Engine;

namespace Tax.Calculator.Helper
{
    /// <summary>
    /// This class is to Calculate tax for the given Vehicle and for the given list of travel dates.
    /// </summary>
    public class CongestionTaxCalculator
    {

        #region Global Varibale Declaration

        private readonly TaxRuleEngine _taxRuleEngine;
        private CityTaxRules _cityTaxRules = new CityTaxRules();

        #endregion

        #region Constructor
        public CongestionTaxCalculator()
        {
            _taxRuleEngine = new TaxRuleEngine();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get tax for the given vehicle and given list of travel dates
        /// </summary>
        /// <param name="vehicle">Given Vehicle</param>
        /// <param name="travelDates">list of travel dates</param>
        /// <returns>Tax to be Paid</returns>
        public int GetTax(IVehicle vehicle, TravelDates travelDates)
        {
            int finalTotalFee = 0;

            // If no travel dates provided, return 0.
            if (travelDates.TravelDateList.Count == 0)
            {
                return finalTotalFee;
            }

            //RunTaxRuleEngine for the given City
            _cityTaxRules = _taxRuleEngine.RunTaxRuleEngine();

            //Convert the String date to Date
            var travelDateList = ConvertStringListToDate(travelDates);

            //Sort the list of travel dates in ascending order
            travelDateList.Sort((x, y) => x.CompareTo(y));
            
            //Segregate the list of dates into list of objects having dates for each unique date.
            var dayTravelList = GetDayTravelList(travelDateList);

            foreach (var dayInstance in dayTravelList)
            {
                var tempFee = 0;
                var totalFee = 0;
                var dayList = dayInstance.DateList;
                var intervalStart = dayList.First();
                foreach (var dayTime in dayList)
                {
                    int nextFee = GetTollFee(dayTime, vehicle);

                    TimeSpan span = dayTime.Subtract(intervalStart);
                    var minutes = span.TotalMinutes;

                    // For a given day, if the travel dates is within 60 mints is only taxed once.
                    // The amount that must be paid is the highest one.
                    if (minutes <= 60)
                    {
                        if (totalFee > 0) totalFee -= tempFee;
                        if (nextFee >= tempFee) tempFee = nextFee;
                    }
                    else
                    {
                        intervalStart = dayTime;
                        tempFee = nextFee;
                    }

                    totalFee += tempFee;
                }
                //For a day, if the Total tax to be paid in greater than 60, then set To
                if (totalFee > 60)
                {
                    totalFee = 60;
                }

                finalTotalFee += totalFee;
            }
            return finalTotalFee;
        }

        #endregion

        #region Private Methods


        /// <summary>
        /// Convert the List of String date to Date List
        /// </summary>
        /// <param name="travelDates"></param>
        /// <returns></returns>
        private List<DateTime> ConvertStringListToDate(TravelDates travelDates)
        {
            var dateList = new List<DateTime>();
            foreach (var travelDate in travelDates.TravelDateList)
            {
                DateTime dateTime = Convert.ToDateTime(travelDate);
                dateList.Add(dateTime);
            }
            return dateList;
        }

        /// <summary>
        /// Segregate the list of dates into list of objects having dates for each unique date.
        /// </summary>
        /// <param name="travelDateList"></param>
        /// <returns></returns>
        private List<DayTravel> GetDayTravelList(List<DateTime> travelDateList)
        {
            var dayTravelList = new List<DayTravel>();
            var firstTravelDayDate = travelDateList.FirstOrDefault();
            var dayTravel = new DayTravel()
            {
                DayId = firstTravelDayDate,
                DateList = new List<DateTime>()
            };

            foreach (var travelDate in travelDateList)
            {
                if (travelDate.Date == firstTravelDayDate.Date)
                {
                    dayTravel.DateList.Add(travelDate);
                }
                else
                {
                    dayTravelList.Add(dayTravel);
                    firstTravelDayDate = travelDate;
                    dayTravel = new DayTravel
                    {
                        DayId = firstTravelDayDate,
                        DateList = new List<DateTime>
                        {
                            firstTravelDayDate
                        }
                    };
                }

            }
            dayTravelList.Add(dayTravel);
            return dayTravelList;
        }

        /// <summary>
        /// Check whether the given vehicle is Toll Free Vehicle
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        private bool IsTollFreeVehicle(IVehicle vehicle)
        {
            if (vehicle == null) return false;
            String vehicleType = vehicle.GetVehicleType();
            return vehicleType.Equals(TollFreeVehicles.Motorcycle.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Bus.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Emergency.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Diplomat.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Foreign.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Military.ToString());
        }


        /// <summary>
        /// Get the Tax fee for the given vehicle and given Date.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        private int GetTollFee(DateTime date, IVehicle vehicle)
        {
            if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle)) return 0;

            foreach (var rulesTime in _cityTaxRules.CityTaxRuleList)
            {
                if (date.TimeOfDay >= rulesTime.StartTime && date.TimeOfDay <= rulesTime.EndTime)
                {
                    return rulesTime.Amount;
                }
            }

            return 0;
        }


        /// <summary>
        /// Check whether the given Date is Toll Free Date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private Boolean IsTollFreeDate(DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;

            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;

            if (year == 2013)
            {
                if (month == 1 && day == 1 ||
                    month == 3 && (day == 28 || day == 29) ||
                    month == 4 && (day == 1 || day == 30) ||
                    month == 5 && (day == 1 || day == 8 || day == 9 || day == 20) ||
                    month == 6 && (day == 5 || day == 6 || day == 21) ||
                    month == 7 ||
                    month == 10 && day == 31 ||
                    month == 11 && day == 1 ||
                    month == 12 && (day == 24 || day == 25 || day == 26 || day == 30 || day == 31))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Tax Exempt vehicles
        /// </summary>
        private enum TollFreeVehicles
        {
            Motorcycle = 0,
            Emergency = 1,
            Diplomat = 2,
            Foreign = 3,
            Military = 4,
            Bus = 5
        }

        #endregion

    }
}
