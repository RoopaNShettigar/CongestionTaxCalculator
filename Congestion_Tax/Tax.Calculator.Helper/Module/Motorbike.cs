using Tax.Calculator.Helper.Interface;

namespace Tax.Calculator.Helper.Module
{
    public class Motorbike : IVehicle
    {
        public string GetVehicleType()
        {
            return "Motorbike";
        }
    }
}
