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

namespace Uber_Driver.DataModels
{
    public static class LetsRideCredentials
    {
        // public static string WebUrl = "http://192.168.254.38:45457/";
      // public static string WebUrl = "http://192.168.43.2:45459/";
      public static string WebUrl = "http://192.168.0.6:45457/";
      //  public static string WebUrl = "http://letsride-001-site1.btempurl.com/";
        public static string HubUrl = WebUrl + "hub/letsride";
        public static string Token = "";
        public static string SessionName = "driverInfo";
        public static string PackageName = "com.letsride.driver";
    }
}