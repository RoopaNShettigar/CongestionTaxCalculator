using System;
using Tax.Calculator.Helper.Interface;

namespace Tax.Calculator.Helper.Module
{
    public class Car : IVehicle
    {
        public String GetVehicleType()
        {
            return "Car";
        }
    }
}
