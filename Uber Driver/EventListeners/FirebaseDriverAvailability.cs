using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Java.Util;
using Uber_Driver.Helpers;

namespace LetsRide.EventListeners
{
    public class FirebaseDriverAvailability : Java.Lang.Object, IValueEventListener
    {
        FirebaseDatabase database;
        DatabaseReference availablityRef;
        AppData data = new AppData();
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

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                string ride_id = snapshot.Child("ride_id").Value.ToString();
                if (ride_id != "waiting" && ride_id != "timeout" && ride_id != "cancelled")
                {
                    //Ride Assigned
                    RideAssigned?.Invoke(this, new RideAssignedIDEventArgs { RideId = ride_id });
                }
                else if (ride_id == "timeout")
                {
                    // Ride Timeout
                    RideTimedOut?.Invoke(this, new EventArgs());
                }
                else if (ride_id == "cancelled")
                {
                    //ride cancelled
                    RideCancelled?.Invoke(this, new EventArgs());
                }
            }

        }

        public void Create(Android.Locations.Location myLocation)
        {
            database = AppDataHelper.GetDatabase();
            string driverID = data.GetCurrentUser.DriverId.ToString();
            string typeId = data.GetCurrentUser.VehicleTypeId.ToString(); 

            availablityRef = database.GetReference("driversAvailable/" + driverID);
            HashMap location = new HashMap();
            location.Put("latitude", myLocation.Latitude);
            location.Put("longitude", myLocation.Longitude);

            HashMap driverInfo = new HashMap();
            driverInfo.Put("location", location);
            driverInfo.Put("ride_id", "waiting");
            driverInfo.Put("type_id", typeId);


            availablityRef.AddValueEventListener(this);
            availablityRef.SetValue(driverInfo);
        }

        public void RemoveListener()
        {
            availablityRef.RemoveValue();
            availablityRef.RemoveEventListener(this);
            availablityRef = null;
        }


        public void UpdateLocation(Android.Locations.Location mylocation)
        {
            string DriverId = data.GetCurrentUser.DriverId.ToString();
            if (availablityRef != null)
            {
                DatabaseReference locationref = database.GetReference("driversAvailable/" + DriverId + "/location");
                HashMap locationMap = new HashMap();
                locationMap.Put("latitude", mylocation.Latitude);
                locationMap.Put("longitude", mylocation.Longitude);
                locationref.SetValue(locationMap);
            }
        }

        public void ReActivate()
        {
            availablityRef.Child("ride_id").SetValue("waiting");

        }
    }
}