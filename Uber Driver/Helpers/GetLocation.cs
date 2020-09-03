using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Uber_Driver.Helpers;

namespace LetsRide.Helpers
{
    public class GetLocation
    {
        //Location Client
        LocationRequest mLocationRequest;
        FusedLocationProviderClient locationProviderClient;
        Android.Locations.Location mLastlocation;
        LocationCallbackHelper mLocationCallback = new LocationCallbackHelper();

        static int UPDATE_INTERVAL = 5; //Seconds
        static int FASTEST_INTERVAL = 5; //Seconds
        static int DISPLACEMENT = 1; //METRES;
        Context _context;

        public event EventHandler<OnLocationCaptionEventArgs> MyLocation;

        public class OnLocationCaptionEventArgs : EventArgs
        {
            public Android.Locations.Location Location { get; set; }
        }
        public GetLocation(Context context)
        {
            this._context = context;
        }
        void CreateLocationRequest()
        {

            mLocationRequest = new LocationRequest();
            mLocationRequest.SetInterval(UPDATE_INTERVAL);
            mLocationRequest.SetFastestInterval(FASTEST_INTERVAL);
            mLocationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            mLocationRequest.SetSmallestDisplacement(DISPLACEMENT);
            mLocationCallback.MyLocation += MLocationCallback_MyLocation;
            locationProviderClient = LocationServices.GetFusedLocationProviderClient(_context);
        }

        private void MLocationCallback_MyLocation(object sender, LocationCallbackHelper.OnLocationCaptionEventArgs e)
        {
            MyLocation.Invoke(this, new OnLocationCaptionEventArgs { Location = e.Location });
        }
        public void StartLocationUpdates()
        {
            CreateLocationRequest();
            locationProviderClient.RequestLocationUpdates(mLocationRequest, mLocationCallback, null);
        }
        void StopLocationUpdates()
        {
            locationProviderClient.RemoveLocationUpdates(mLocationCallback);
        }
    }
}