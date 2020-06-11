using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace LetsRide.DataModels.Data
{
    public static class Status
    {
        public  const  string COMPLETED ="COMPLETE";
        public const string ARRIVED = "ARRIVED";
        public const string ONTRIP = "ONTRIP";
        public const string NORESPONSE = null;
        public const string ACCEPTED = "ACCEPTED";
        public const string CANCELLED = "CANCELLED";

    }
}