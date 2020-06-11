using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Uber_Driver.DataModels
{
    public class DriverInfomation
    {
        public string DriverName { get; set; }
        public decimal DriverPhoneNumber { get; set; }
        public int TypeId { get; set; }
        public Guid DriverId { get; set; }
        public string DriverEmail { get; set; }
        public string Password { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public int VehicleTypeId { get; set; }
        public string Token { get; set; }
        public decimal OTP { get; set; }
    }
}