using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Gms.Location;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Uber_Driver.DataModels;
using Uber_Driver.Helpers;
using Uber_Driver.WebServices;

namespace LetsRide.Broadcast
{
    [BroadcastReceiver]
    public class LocationService : BroadcastReceiver
    {
        public static string ACTION_PROCESS_LOCATION = "LetsRide.UPDATE.LOCATION";
        public const string PRIMARY_CHANNEL = "Urgent";
        public const int NOTIFY_ID = 100;
        public override async void OnReceive(Context context, Intent intent)
        {
          if(intent != null)
            {
                string action = intent.Action;
                if (action.Equals(ACTION_PROCESS_LOCATION))
                {
                    LocationResult locationResult = LocationResult.ExtractResult(intent);
                    if (locationResult != null)
                    {
                        var location = locationResult.LastLocation;
                        string str = "Location Updated/" + location.Longitude + "/" + location.Latitude;
                        await UpdateLocation(new LatLng(location.Latitude, location.Longitude));
                       // Notifications(str, context, intent);
                    }
                }
            }
        }

        public async Task<ResponseData> UpdateLocation(LatLng myLocation)
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
        private void Notifications(string displayText, Context context, Intent intent)
        {

            NotificationManager manager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            // manager.Notify(1, builder.Build());


            string channelName = "Secondary Channel";
            var importance = NotificationImportance.High;
            var channel = new NotificationChannel(PRIMARY_CHANNEL, channelName, importance);
            channel.EnableLights(true);
            channel.EnableLights(true);
            channel.LockscreenVisibility = NotificationVisibility.Public;

            manager.CreateNotificationChannel(channel);
            intent.AddFlags(ActivityFlags.SingleTop);
            PendingIntent pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.CancelCurrent);

            Notification.Builder builder = new Notification.Builder(context)
                .SetContentTitle("Lets Ride")
                .SetSmallIcon(Resource.Drawable.centerimage)
                .SetContentText(displayText)
                .SetChannelId(PRIMARY_CHANNEL)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);
            manager.Notify(NOTIFY_ID, builder.Build());
        }
    }
}