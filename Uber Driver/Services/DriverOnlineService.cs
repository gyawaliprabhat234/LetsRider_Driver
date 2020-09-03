using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using LetsRide.DataModels.Data;
using LetsRide.EventListeners;
using LetsRide.Helpers;
using Uber_Driver;
using Uber_Driver.Helpers;
using Xamarin.Essentials;

namespace LetsRide.Services
{
    [Service]
    public class DriverOnlineService : Service
    {

    
        public const string PRIMARY_CHANNEL = "Urgent";
        public const int NOTIFY_ID = Constants.SERVICE_RUNNING_NOTIFICATION_ID;
        public static string Status;
        static readonly int TimerWait = 5000;
        static readonly string TAG = "X:" + typeof(DriverOnlineService).Name;
        Timer timer;
        DateTime startTime;
        Android.Locations.Location mLastLocation;
        bool isStarted = false;
        bool IsConnectionAvailable = false;
        bool hasLocationSet = false;
        FirebaseDriverAvailability driverAvailability;
        public override void OnCreate()
        {
            driverAvailability = new FirebaseDriverAvailability();
           
            base.OnCreate();


        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        private void Initialization()
        {
            SetConnection();
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            NotificationManager manager = (NotificationManager)GetSystemService(NotificationService);
            // manager.Notify(1, builder.Build());


            string channelName = "Secondary Channel";
            var importance = NotificationImportance.High;
            var channel = new NotificationChannel(PRIMARY_CHANNEL, channelName, importance);
            channel.EnableLights(true);
            channel.LockscreenVisibility = NotificationVisibility.Public;

            manager.CreateNotificationChannel(channel);

             Intent intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.SingleTop);
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.CancelCurrent);
            Notification.Builder notification = new Notification.Builder(this, PRIMARY_CHANNEL)
                .SetContentTitle("LetsRide")
                .SetSmallIcon(Resource.Drawable.centerimage)
                .SetContentText("You are online Now")
                .SetContentIntent(pendingIntent);

            StartForeground(NOTIFY_ID, notification.Build());
            

        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
           if(e.NetworkAccess == NetworkAccess.Internet)
            {
                IsConnectionAvailable = true;
            }
            else
            {
                IsConnectionAvailable = false;
            }
        }
        private void SetConnection()
        {
            if (IsConnectionAvailable)
                return ;
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                IsConnectionAvailable = true;
            else
                IsConnectionAvailable = false;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            GetLocation getLocation = new GetLocation(this);
            if (intent.Action.Equals(Constants.ACTION_START_SERVICE))
            {
                Initialization();

               
                Log.Debug(TAG, $"OnStartCommand called at {startTime}, flags={flags}, startid={startId}");
                if (isStarted)
                {
                    TimeSpan runtime = DateTime.UtcNow.Subtract(startTime);
                    Log.Debug(TAG, $"This service was already started, it's been running for {runtime:c}.");
                }
                else
                {
                    try
                    {
                        while (!IsConnectionAvailable) { }
                        getLocation.StartLocationUpdates();
                        getLocation.MyLocation += GetLocation_MyLocation; ;
                        //startTime = DateTime.UtcNow;
                        //Log.Debug(TAG, $"Starting the service, at {startTime}.");
                        //timer = new Timer(HandleTimerCallback, startTime, 0, TimerWait);
                        isStarted = true;
                    }catch(Exception ex)
                    {

                    }
                }
            }
            else if (intent.Action.Equals(Constants.ACTION_STOP_SERVICE))
            {
                getLocation.StartLocationUpdates();
                StopForeground(true);
                StopSelf();
                isStarted = false;

            }
           
            return StartCommandResult.NotSticky;
        }

        private void GetLocation_MyLocation(object sender, GetLocation.OnLocationCaptionEventArgs e)
        {
            while (!IsConnectionAvailable) { }
            mLastLocation = e.Location;
            if (!hasLocationSet)
            {
                driverAvailability.Create(mLastLocation);
                hasLocationSet = true;
            }
            else
                driverAvailability.UpdateLocation(mLastLocation);
        }

       
        private void HandleTimerCallback(object state)
        {
            TimeSpan runTime = DateTime.UtcNow.Subtract(startTime);
            Log.Debug(TAG, $"This service has been running for {runTime:c} (since ${state}).");
            LatLng location = new LatLng(25, 24);
          //  driverAvailability.UpdateLocation(location);
        }
    }
}