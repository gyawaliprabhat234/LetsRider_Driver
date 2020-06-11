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
using LetsRide.DataModels.Rides;

namespace LetsRide.DataModels
{
    public class DriverDetailsInfo
    {
        public string FullNameEnglish { get; set; }
        public string FullNameNepali { get; set; }
        public string TemporaryAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string DriverImagePath { get; set; }
        public string Email { get; set; }
        public decimal PhoneNumber { get; set; }
        public RidesReport RideReport { get; set; }

    }
}