using Microsoft.AspNetCore.Mvc;
using Tax.Calculator.Helper;
using Tax.Calculator.Helper.Interface;
using Tax.Calculator.Helper.Module;
using Tax.Entity.Common;

namespace Tax.Calculator.Service.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TaxController : ControllerBase
    {
        private readonly CongestionTaxCalculator _congestionTaxCalculator;
        public TaxController()
        {
            _congestionTaxCalculator = new CongestionTaxCalculator();
        }

        [HttpGet]
        public int GetTaxForCar([FromBody] TravelDates travelDates)
        {
            IVehicle car = new Car();
            return _congestionTaxCalculator.GetTax(car, travelDates);
        }

        [HttpGet]
        public int GetTaxForMotorbike([FromBody] TravelDates travelDates)
        {
            IVehicle motorbike = new Motorbike();
            return _congestionTaxCalculator.GetTax(motorbike, travelDates);
        }
    }
}
