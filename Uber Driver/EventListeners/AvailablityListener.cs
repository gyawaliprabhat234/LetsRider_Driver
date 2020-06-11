using System;
using System.Threading.Tasks;
using Firebase.Database;
using Java.Util;
using Microsoft.AspNetCore.SignalR.Client;
using Uber_Driver.DataModels;
using Uber_Driver.Helpers;
using Uber_Driver.WebServices;

namespace Uber_Driver.EventListeners
{
    public class AvailablityListener 
    {
      
        // FirebaseDatabase database;
        // DatabaseReference availablityRef;
        public class RideAssignedIDEventArgs : EventArgs
        {
            public string RideId { get; set; }
        }
        public event EventHandler<RideAssignedIDEventArgs> RideAssigned;
        public event EventHandler RideCancelled;
        public event EventHandler RideTimedOut;
        public void OnCancelled(DatabaseError error)
        {
        }
        public void OnRideRequest(RideDetails ride)
        {

        }
        public void OnRideRequestCancelOrTimeout(string status)
        {
            if(status == "timeout")
            {
                RideTimedOut?.Invoke(this, new EventArgs());
            }
            else if (status == "cancel")
            {
                RideCancelled?.Invoke(this, new EventArgs());
            }

            //if(snapshot.Value != null)
            //{
            //    string ride_id = snapshot.Child("ride_id").Value.ToString();
            //    if(ride_id != "waiting" && ride_id != "timeout" && ride_id != "cancelled")
            //    {
            //        //Ride Assigned
            //        RideAssigned?.Invoke(this, new RideAssignedIDEventArgs { RideId = ride_id });
            //    }
            //    else if (ride_id == "timeout")
            //    {
            //        // Ride Timeout
            //        RideTimedOut?.Invoke(this, new EventArgs());
            //    }
            //    else if (ride_id == "cancelled")
            //    {
            //        //ride cancelled
            //        RideCancelled?.Invoke(this, new EventArgs());
            //    }
            //}

        }
        public async Task<ResponseData> Create (Android.Locations.Location myLocation, HubConnection connection)
        {
            try
            {
                ResponseData response = new ResponseData();
                ActiveDrivers activeDrivers = new ActiveDrivers()
                {
                    Action = "A",
                    Longitude = Convert.ToDecimal(myLocation.Longitude),
                    Latitude = Convert.ToDecimal(myLocation.Latitude),
                    IsOnline = false
                };

                return await new AvailabilityService().ActiveDriver(activeDrivers);
            }
            catch (Exception ex)
            {

                return new ResponseData() { IsSuccess = false, Message = ex.Message };
            }
            

            //database = AppDataHelper.GetDatabase();
            //string driverID = AppDataHelper.GetCurrentUser().Uid;
            //string typeId = AppDataHelper.GetTypeId();

            //availablityRef = database.GetReference("driversAvailable/" + driverID);
            //HashMap location = new HashMap();
            //location.Put("latitude", myLocation.Latitude);
            //location.Put("longitude", myLocation.Longitude);
            //HashMap driverInfo = new HashMap();
            //driverInfo.Put("location", location);
            //driverInfo.Put("ride_id", "waiting");
            //driverInfo.Put("type_id", typeId);
            //availablityRef.AddValueEventListener(this);
            //availablityRef.SetValue(driverInfo);
        }
        public async Task<ResponseData> SetBusy()
        {
            ResponseData response = new ResponseData();
            ActiveDrivers activeDrivers = new ActiveDrivers()
            {
                Action = "B",
               
            };
            return await new AvailabilityService().ActiveDriver(activeDrivers);
        }
        public async Task<ResponseData> UpdateLocation(Android.Locations.Location myLocation)
        {
            ResponseData response = new ResponseData();
            ActiveDrivers activeDrivers = new ActiveDrivers()
            {
                Action = "L",
                Longitude = Convert.ToDecimal(myLocation.Longitude),
                Latitude = Convert.ToDecimal(myLocation.Latitude)
            };
            return await new AvailabilityService().ActiveDriver(activeDrivers);
        }
        public async Task<ResponseData> ReActivate()
        {
            ResponseData response = new ResponseData();
            ActiveDrivers activeDrivers = new ActiveDrivers()
            {
                Action = "S"
            };
         return   await new AvailabilityService().ActiveDriver(activeDrivers);

        }

    }
}
