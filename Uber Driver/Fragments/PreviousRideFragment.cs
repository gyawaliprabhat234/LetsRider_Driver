using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using LetsRide.DataModels.Rides;

namespace LetsRide.Fragments
{
    public class PreviousRideFragment : Android.Support.V4.App.DialogFragment
    {
        RelativeLayout completeButton;
        RelativeLayout cancelButton;
        TextView pickupAddressText;
        TextView destinationAddressText;
        TextView timeDuration;
        TextView totalDistance;
        TextView errorText;
        ImageView callBtn;
        Rides rides;
        Android.Support.Design.Widget.TextInputLayout reasonInputText;
        TextView statusText;
        public class EventParameter : EventArgs {
            public string Reason { get; set; }
        }

        public event EventHandler Completed;
        public event EventHandler<EventParameter> Cancelled;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }
        public PreviousRideFragment(Rides rides)
        {
            this.rides = rides;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.previous_assigned_ride, container, false);
            pickupAddressText = (TextView)view.FindViewById(Resource.Id.newridePickupText);
            destinationAddressText = (TextView)view.FindViewById(Resource.Id.newrideDestinationText);
            totalDistance = (TextView)view.FindViewById(Resource.Id.totalDistance);
            statusText = (TextView)view.FindViewById(Resource.Id.statusText);
            timeDuration = (TextView)view.FindViewById(Resource.Id.timeDuration);
            reasonInputText = (Android.Support.Design.Widget.TextInputLayout)view.FindViewById(Resource.Id.reasonText);
            completeButton = (RelativeLayout)view.FindViewById(Resource.Id.completeButton);
            cancelButton = (RelativeLayout)view.FindViewById(Resource.Id.cancelButton);
            errorText = (TextView)view.FindViewById(Resource.Id.errorTextMessage);
            callBtn = (ImageView)view.FindViewById(Resource.Id.callBtn);
            errorText.Visibility = ViewStates.Gone;
            TimeSpan time = TimeSpan.FromSeconds(Convert.ToDouble(rides.RidesInfo.EstimatedArrivalTime));
            timeDuration.Text = "Estimated " + ((time.Hours > 0) ? (time.Hours + " hours ") : "") + time.Minutes + " mins";
            totalDistance.Text = Math.Round(Convert.ToDouble(rides.RidesInfo.LocationInfo.TotalDistance) / 1000).ToString() + " km";
            pickupAddressText.Text = "From : " + rides.RidesInfo.LocationInfo.PickupLocationName;
            destinationAddressText.Text ="To : " + rides.RidesInfo.LocationInfo.PickupDestinationName;
            statusText.Text = string.IsNullOrEmpty(rides.RidesInfo.RideStatus) ? "None": rides.RidesInfo.RideStatus +"-" + rides.RideId.ToString();
            completeButton.Click += CompleteButton_Click;
            cancelButton.Click += CancelButton_Click;
            callBtn.Click += CallBtn_Click;
            return view;
        }

        private void CallBtn_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("tel:" + rides.CustomerPhoneNumber.ToString().Split(".")[0]);
            Intent intent = new Intent(Intent.ActionDial, uri);
            StartActivity(intent);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
           if(reasonInputText.EditText.Text.Length > 10)
            {
                Cancelled.Invoke(this, new EventParameter { Reason = reasonInputText.EditText.Text });
            }
            else
            {
                errorText.Visibility = ViewStates.Visible;
                errorText.Text = "Please specify a valid reason to cancel.";
            }
            
        }

        private void CompleteButton_Click(object sender, EventArgs e)
        {
            Completed.Invoke(this, new EventArgs());
        }
    }
}