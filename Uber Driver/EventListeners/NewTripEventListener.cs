using System;
using System.Threading.Tasks;
using Firebase.Database;
using Uber_Driver.DataModels;
using Uber_Driver.Helpers;
using Uber_Driver.WebServices;

namespace Uber_Driver.EventListeners
{
    public class NewTripEventListener 
    {
        string mRideID;
        //Android.Locations.Location mLastlocation;
        //FirebaseDatabase database;
        //DatabaseReference tripRef;

        //flag
        bool isAccepted;
        public NewTripEventListener(string ride_id)
        {
            mRideID = ride_id;
          //  mLastlocation = lastlocation;
            //database = AppDataHelper.GetDatabase();
        }

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            //if(snapshot.Value != null)
            //{
            //    if (!isAccepted)
            //    {
            //        isAccepted = true;
            //        Accept();
            //    }
            //}
        }

      

    
        public void UpdateLocation(Android.Locations.Location lastlocation)
        {
            //mLastlocation = lastlocation;
            //tripRef.Child("driver_location").Child("latitude").SetValue(mLastlocation.Latitude);
            //tripRef.Child("driver_location").Child("longitude").SetValue(mLastlocation.Longitude);

        }

        public async Task<ResponseData> UpdateStatus(string status)
        {
            return await new TripService().UpdatesRideInfo(new RidesInfo()
            {
                Action = status, 
                RideId = Guid.Parse(mRideID)
            });
        }

        public void EndTrip (double fares)
        {

            //Update: Calls the garbage collector to release instances existing in memory. This hanles error: Invalid Instance. 
         //   GC.Collect();


            //if(tripRef != null)
            //{ 
            //    tripRef.Child("fares").SetValue(fares);
            //    tripRef.Child("status").SetValue("ended");
            //    tripRef.RemoveEventListener(this);
            //    tripRef = null;

            //}
        }
    }
}
