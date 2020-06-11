using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using LetsRide;
using LetsRide.DataModels.Data;
using LetsRide.DataModels.Rides;
using Uber_Driver.WebServices;

namespace Uber_Driver.Fragments
{
    public class RidesAdapter : RecyclerView.Adapter //Android.Support.V4.App.Fragment
    {

        //public override void OnCreate(Bundle savedInstanceState)
        //{
        //    base.OnCreate(savedInstanceState);

        //    // Create your fragment here
        //}

        //public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        //{
        //    // Use this to return your custom view for this Fragment
        //    View view = inflater.Inflate(Resource.Layout.earnings, container, false);

        //    return view;
        //}
        List<RideDetailsInfo> Items;
        public RidesAdapter(List<RideDetailsInfo> rideList)
        {
            Items = rideList;
            
        }
        public event EventHandler<AdapterClickEventArgs> ItemClick;
        public event EventHandler<AdapterClickEventArgs> ItemLongClick;
        public event EventHandler<AdapterClickEventArgs> SelectItemClick;
        public override int ItemCount => Items.Count;
        void OnClick(AdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(AdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        void OnSelectClick(AdapterClickEventArgs args) => SelectItemClick?.Invoke(this, args);
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            RideDetailsInfo rideInfo = Items[position];

            // Replace the contents of the view with that element
            var h = holder as AdapterViewHolder;
            //holder.TextView.Text = items[position];
            h.date.Text = rideInfo.RideBookingTime.ToString("ddd, dd MMM yyyy");
            h.time.Text =rideInfo.RideBookingTime.ToString("hh:mm tt");
            h.newridePickupText.Text = "From : "+ rideInfo.PickupLocationName;
            h.newrideDestinationText.Text ="To : "+ rideInfo.PickupDestinationName;
            h.status.Text = rideInfo.RideStatus;
            switch (rideInfo.RideStatus)
            {
                case Status.COMPLETED:
                    h.status.SetTextColor(Color.Green);
                    break;

                case Status.CANCELLED:
                    h.status.SetTextColor(Color.Red);
                    break;


            }
            h.earnings.Text = "NPR. "+ rideInfo.TotalCost.ToString();
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.trip_info, parent, false);
            //var id = Resource.Layout.__YOUR_ITEM_HERE;
            //itemView = LayoutInflater.From(parent.Context).
            //       Inflate(id, parent, false);

            var vh = new AdapterViewHolder(itemView, OnClick, OnLongClick, OnSelectClick);
            return vh;
        }
        public class AdapterViewHolder : RecyclerView.ViewHolder
        {
            //public TextView TextView { get; set; }
            public TextView date { get; set; }
            public TextView time { get; set; }
            public TextView newridePickupText { get; set; }
            public TextView newrideDestinationText { get; set; }
            public TextView status { get; set; }
            public TextView earnings { get; set; }

            public AdapterViewHolder(View itemView, Action<AdapterClickEventArgs> clickListener,
                                Action<AdapterClickEventArgs> longClickListener, Action<AdapterClickEventArgs> selectClickListener) : base(itemView)
            {
                //TextView = v;
                date = (TextView)itemView.FindViewById(Resource.Id.date);
                time = (TextView)itemView.FindViewById(Resource.Id.time);
                newridePickupText = (TextView)itemView.FindViewById(Resource.Id.newridePickupText);
                newrideDestinationText = (TextView)itemView.FindViewById(Resource.Id.newrideDestinationText);
                // selectButton = (Button)itemView.FindViewById(Resource.Id.selected);
                status = (TextView)itemView.FindViewById(Resource.Id.status);
                earnings = (TextView)itemView.FindViewById(Resource.Id.earnings);

                itemView.Click += (sender, e) => clickListener(new AdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new AdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                //  selectButton.Click += (sender, e) => selectClickListener(new VehicleAdapterClickEventArgs() { View = itemView, Position = AdapterPosition });

            }
        }
        public class AdapterClickEventArgs : EventArgs
        {
            public View View { get; set; }
            public int Position { get; set; }
        }

    }
}