using System;
using System.Collections.Generic;
using System.Text;

namespace LetsRide.DataModels.Rides
{
   public class RidesLocationInfo
   {
        public string Action { get; set; }
        public System.Guid LocationId { get; set; }
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
