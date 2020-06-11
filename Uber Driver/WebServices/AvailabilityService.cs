using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Uber_Driver.DataModels;
using Uber_Driver.Helpers;

namespace Uber_Driver.WebServices
{
   public class AvailabilityService
    {
        public async Task<ResponseData> ActiveDriver(ActiveDrivers activeDrivers)
        {
            AppData userInfo = new AppData();
            if (userInfo.GetCurrentUser.DriverId != new Guid() && !string.IsNullOrEmpty(userInfo.GetToken))
            {
                DriverInfomation driverInformation = userInfo.GetCurrentUser;
                activeDrivers.DriverId = driverInformation.DriverId;
                activeDrivers.VehicleTypeId = driverInformation.VehicleTypeId;
                string drivers = JsonConvert.SerializeObject(activeDrivers);
                return await Common.WebPostService("api/Driver/ActiveDriver",drivers , true);

            }
            return new ResponseData() { IsSuccess = false };

        }

        public async Task<ResponseData> DriverDetails()
        {
            AppData userInfo = new AppData();
            if (userInfo.GetCurrentUser.DriverId != new Guid() && !string.IsNullOrEmpty(userInfo.GetToken))
            {
                DriverInfomation driverInformation = userInfo.GetCurrentUser;
                ActiveDrivers driver = new ActiveDrivers() { DriverId = driverInformation.DriverId };
                string drivers = JsonConvert.SerializeObject(driver);
                return await Common.WebPostService("api/Driver/GetDriverDetails", drivers, true);

            }
            return new ResponseData() { IsSuccess = false };

        }
    }
}