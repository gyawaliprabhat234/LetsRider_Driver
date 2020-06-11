using System;
using System.Collections.Generic;
using System.Text;

namespace LetsRide.DataModels.Rides
{
    public class Rides
    {
        public string Action { get; set; }
        public System.Guid RideId { get; set; }
        public byte RideTypeId { get; set; }
        public Nullable<System.Guid> DriverId { get; set; }
        public Nullable<System.Guid> CustomerId { get; set; }
        public string DriverName { get; set; }
        public Nullable<decimal> DriverPhoneNumber { get; set; }
        public Nullable<decimal> CustomerPhoneNumber { get; set; }
        public string CustomerName { get; set; }
        public string VehicleName { get; set; }
        public int VehicleTypeId { get; set; }
    //    public List<RideRequest> RideRequests { get; set; }
        public RideType RideType { get; set; }
        public RidesInfo RidesInfo { get; set; }




    }
}
