
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
using LetsRide;
using Uber_Driver.DataModels;

namespace Uber_Driver.Fragments
{
    public class NewRequestFragment : Android.Support.V4.App.DialogFragment
    {

        //Views
        RelativeLayout acceptRideButton;
        RelativeLayout rejectRideButton;
        TextView pickupAddressText;
        TextView destinationAddressText;
        TextView timeDuration;
        TextView totalDistance;

        RideDetails rideDetails;

        //Events
        public event EventHandler RideAccepted;
        public event EventHandler RideRejected;

        public NewRequestFragment(RideDetails rideDetails)
        {
            this.rideDetails = rideDetails;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
           View view =   inflater.Inflate(Resource.Layout.newrequest_dialogue, container, false);
            pickupAddressText = (TextView)view.FindViewById(Resource.Id.newridePickupText);
            destinationAddressText = (TextView)view.FindViewById(Resource.Id.newrideDestinationText);
            totalDistance = (TextView)view.FindViewById(Resource.Id.totalDistance);
            timeDuration = (TextView)view.FindViewById(Resource.Id.timeDuration);
            TimeSpan time = TimeSpan.FromSeconds(Convert.ToDouble(rideDetails.EstimatedArrivalTime));
            pickupAddressText.Text = "From : " +  rideDetails.PickupAddress;
            destinationAddressText.Text = "To : "+ rideDetails.DestinationAddress;
            timeDuration.Text ="Estimated " + ((time.Hours > 0) ? (time.Hours + " hours "): "") + time.Minutes + " mins";
            totalDistance.Text = Math.Round(rideDetails.Distance/1000).ToString() + " km";
            acceptRideButton = (RelativeLayout)view.FindViewById(Resource.Id.acceptRideButton);
            rejectRideButton = (RelativeLayout)view.FindViewById(Resource.Id.rejectRideButton);
            acceptRideButton.Click += AcceptRideButton_Click;
            rejectRideButton.Click += RejectRideButton_Click;

            return view;
        }

        void AcceptRideButton_Click(object sender, EventArgs e)
        {
            RideAccepted?.Invoke(this, new EventArgs());
        }

        void RejectRideButton_Click(object sender, EventArgs e)
        {
            RideRejected?.Invoke(this, new EventArgs());
        }

    }
}
