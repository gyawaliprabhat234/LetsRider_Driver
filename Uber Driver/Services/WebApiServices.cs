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
using Uber_Driver.DataModels;
using Uber_Driver.Helpers;

namespace Uber_Driver.Services
{
    public class WebApiServices
    {
        public DriverInfomation GetDriverInformation(string email, string password)
        {
            return new DriverInfomation()
            {
                DriverName = "Prabhat Gyawali"
               ,
                DriverPhoneNumber = 9867122118,
                TypeId = 1
            };
        }
    }
}