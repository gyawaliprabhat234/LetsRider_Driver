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
   public   class LoginService
    {
        public async Task<ResponseData> CheckDriverExist(DriverInfomation driverInformation)
        {
            string driverInfo = JsonConvert.SerializeObject(driverInformation);
            return await Common.WebPostService("api/Login/CheckDriverExist", driverInfo, false);
        }

        public async Task<ResponseData> CheckOtpCodeForDriver(DriverInfomation driverInformation)
        {
            string driverInfo = JsonConvert.SerializeObject(driverInformation);
            return await Common.WebPostService("api/Login/CheckOtpCodeForDriver", driverInfo, false);
        }
        public async Task<ResponseData> CheckDriverLoginInfo(DriverInfomation driverInformation)
        {
            string driverInfo = JsonConvert.SerializeObject(driverInformation);
            return await Common.WebPostService("api/Login/CheckDriverLoginInfo", driverInfo, false);
        }

        public async Task<ResponseData> ResendVerificationCode(DriverInfomation driverInformation)
        {
            string driverInfo = JsonConvert.SerializeObject(driverInformation);
            return await Common.WebPostService("api/Login/ResendDriverVerificationCode", driverInfo, false);
        }

        public async Task<ResponseData> IsDriverLoggedIn()
        {
            AppData userInfo = new AppData();
           if(userInfo.GetCurrentUser.DriverId != new Guid() && !string.IsNullOrEmpty(userInfo.GetToken))
            {
                DriverInfomation driverInformation = userInfo.GetCurrentUser;
                string driverInfo = JsonConvert.SerializeObject(driverInformation);
                return await Common.WebPostService("api/Login/IsDriverLoggedIn", driverInfo, true);

            }
            return new ResponseData() { IsSuccess = false };
           
        }
    }
}