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
using LetsRide.DataModels;
using Newtonsoft.Json;
using Uber_Driver.DataModels;
using Uber_Driver.WebServices;

namespace Uber_Driver.Fragments
{
    public class AccountFragment : Android.Support.V4.App.Fragment
    {
        ImageView refresh;
        ImageView profilePic;
        TextView fullname;
        TextView nepaliname;
        TextView totalRides;
        TextView totalEarnings;
        TextView completeRides;
        TextView cancelledRides;
        TextView email;
        TextView phone;
        TextView permanent_address;
        TextView temporary_address;
        public event EventHandler OnProgress;
        public event EventHandler EndProgress;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.account, container, false);
            refresh = (ImageView)view.FindViewById(Resource.Id.refresh);
            profilePic = (ImageView)view.FindViewById(Resource.Id.profilePic);
            fullname = (TextView)view.FindViewById(Resource.Id.fullname);
            nepaliname = (TextView)view.FindViewById(Resource.Id.nepaliname);
            totalRides = (TextView)view.FindViewById(Resource.Id.totalRides);
            totalEarnings = (TextView)view.FindViewById(Resource.Id.totalEarnings);
            completeRides = (TextView)view.FindViewById(Resource.Id.completeRides);
            cancelledRides = (TextView)view.FindViewById(Resource.Id.cancelledRides);
            email = (TextView)view.FindViewById(Resource.Id.email);
            phone = (TextView)view.FindViewById(Resource.Id.phone);
            permanent_address = (TextView)view.FindViewById(Resource.Id.permanent_address);
            temporary_address = (TextView)view.FindViewById(Resource.Id.temporary_address);
            refresh.Click += Refresh_Click;
            SetDriverDetails();

            return view;
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
           SetDriverDetails();
        }

        async void SetDriverDetails()
        {
            OnProgress.Invoke(this, new EventArgs());
            ResponseData response = await new AvailabilityService().DriverDetails();
            DriverDetailsInfo driverInfo = JsonConvert.DeserializeObject<DriverDetailsInfo>(response.RecordsInString);
            profilePic.SetImageBitmap(Common.GetImageBitmapFromUrl(LetsRideCredentials.WebUrl + driverInfo.DriverImagePath));
            fullname.Text = driverInfo.FullNameEnglish;
            nepaliname.Text = driverInfo.FullNameNepali;
            totalRides.Text = driverInfo.RideReport.TotalRides.ToString();
            totalEarnings.Text = "NPR." + driverInfo.RideReport.TotalCost.ToString();
            completeRides.Text = driverInfo.RideReport.Complete.ToString();
            cancelledRides.Text = driverInfo.RideReport.Cancelled.ToString();
            email.Text = driverInfo.Email;
            phone.Text = driverInfo.PhoneNumber.ToString().Split(".")[0];
            permanent_address.Text = driverInfo.PermanentAddress;
            temporary_address.Text = driverInfo.TemporaryAddress;
            EndProgress.Invoke(this, new EventArgs());
        }



    }
}