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

namespace LetsRide.DataModels.Rides
{
    public class RidesReport
    {
        public int TotalRides { get; set; }
        public int NoResponse { get; set; }
        public int Cancelled { get; set; }
        public int Accepted { get; set; }
        public int Arrived { get; set; }
        public int OnTrip { get; set; }
        public int Complete { get; set; }
        public int CompleteUsingSystem { get; set; }
        public int CompleteMannually { get; set; }
        public double TotalCost { get; set; }
    }
}