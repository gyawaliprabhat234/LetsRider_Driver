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
    public class AcceptedDriver
    {
        public string ID { get; set; }
        public string fullname { get; set; }
        public string phone { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string Status { get; set; }
    }
}