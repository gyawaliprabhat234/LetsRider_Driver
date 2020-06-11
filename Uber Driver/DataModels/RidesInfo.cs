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
    public class RidesInfo
    {
        public string Action { get; set; }
         public System.Guid RideId { get; set; }

    }
}