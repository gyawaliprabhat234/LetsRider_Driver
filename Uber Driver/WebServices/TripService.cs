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
using LetsRide.DataModels;
using Newtonsoft.Json;
using Uber_Driver.DataModels;
using Uber_Driver.Helpers;

namespace Uber_Driver.WebServices
{
    public class TripService
    {
        public async Task<ResponseData> UpdatesRideInfo(RidesInfo rideInfo)
        {
            string ride = JsonConvert.SerializeObject(rideInfo);
            return await Common.WebPostService("api/Trip/UpdatesRideInfo", ride, true);
        }

        public async Task<ResponseData> SaveRideRequest(RideRequest rideRequest)
        {
            string ride = JsonConvert.SerializeObject(rideRequest);
            return await Common.WebPostService("api/Driver/SaveRideRequest", ride, true);
        }

        public async Task<List<LetsRide.DataModels.Rides.RideDetailsInfo>> GetRidesInfo()
        {
            List<LetsRide.DataModels.Rides.RideDetailsInfo> rides = new List<LetsRide.DataModels.Rides.RideDetailsInfo>();
            AppData userInfo = new AppData();
            if (userInfo.GetCurrentUser.DriverId != new Guid() && !string.IsNullOrEmpty(userInfo.GetToken))
            {
                DriverInfomation driverInformation = userInfo.GetCurrentUser;
                ActiveDrivers driver = new ActiveDrivers() { DriverId = driverInformation.DriverId };
                string drivers = JsonConvert.SerializeObject(driver);
                ResponseData response = await Common.WebPostService("api/Trip/GetRidesList", drivers, true);
                if (response.IsSuccess)
                {
                    rides = JsonConvert.DeserializeObject<List<LetsRide.DataModels.Rides.RideDetailsInfo>>(response.RecordsInString);
                }
            }
            return rides;
        }

    }
}