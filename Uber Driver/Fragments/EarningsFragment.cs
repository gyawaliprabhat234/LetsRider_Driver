using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using LetsRide;
using LetsRide.DataModels.Rides;
using Uber_Driver.WebServices;
using static Uber_Driver.Fragments.RidesAdapter;

namespace Uber_Driver.Fragments
{
    public class EarningsFragment : Android.Support.V4.App.Fragment
    {
        RecyclerView recyclerView;
        ImageView refresh;
        RidesAdapter adapter;
        List<RideDetailsInfo> rideList;
        public event EventHandler OnProgress;
        public event EventHandler EndProgress;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.earnings, container, false);
            recyclerView = (RecyclerView)view.FindViewById(Resource.Id.myRecyclerView);
            refresh = (ImageView)view.FindViewById(Resource.Id.refresh);
            refresh.Click += Refresh_Click;
            SetUpRecyclerView();
            return view;
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            SetUpRecyclerView();
        }

        private async void SetUpRecyclerView()
        {
            OnProgress.Invoke(this, new EventArgs());
            rideList = await new TripService().GetRidesInfo();
            recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(recyclerView.Context));
            adapter = new RidesAdapter(rideList);
            adapter.ItemClick += Adapter_ItemClick;
            recyclerView.SetAdapter(adapter);
            EndProgress.Invoke(this, new EventArgs());
        }

        private void Adapter_ItemClick(object sender, AdapterClickEventArgs e)
        {

        }
       
    }
}