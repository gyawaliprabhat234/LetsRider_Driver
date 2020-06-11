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
    public class RideDetailsInfo
    {
        public System.Guid RideId { get; set; }
        public byte RideTypeId { get; set; }
        public string RideStatus { get; set; }
        public string Reasons { get; set; }
        public string CustomerName { get; set; }
        public string DriverName { get; set; }
        public Nullable<System.Guid> DriverId { get; set; }
        public Nullable<System.Guid> CustomerId { get; set; }
        public int VehicleTypeId { get; set; }
        public string VehicleName { get; set; }
        public System.DateTime RideBookingTime { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<decimal> EstimatedArrivalTime { get; set; }
        public System.Guid LocationId { get; set; }
        public string RideReview { get; set; }
        public Nullable<byte> RideRating { get; set; }
        public Nullable<decimal> TotalCost { get; set; }
        public Nullable<decimal> PickupLongitude { get; set; }
        public Nullable<decimal> PickupLatitude { get; set; }
        public Nullable<decimal> DestinationLongitude { get; set; }
        public Nullable<decimal> DestinationLatitude { get; set; }
        public string PickupLocationName { get; set; }
        public string PickupDestinationName { get; set; }
        public Nullable<decimal> TotalDistance { get; set; }
        public Nullable<decimal> DriverLatitude { get; set; }
        public Nullable<decimal> DriverLongitude { get; set; }
    }
}