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
using Newtonsoft.Json;
using Uber_Driver.DataModels;

namespace Uber_Driver.Helpers
{
    public class AppData
    {
        ISharedPreferences preferences = Application.Context.GetSharedPreferences(LetsRideCredentials.SessionName, FileCreationMode.Private);
        ISharedPreferencesEditor editor;
        public void SetUserInformation(DriverInfomation user, string token)
        {
            editor = preferences.Edit();
            editor.PutString("userinfo", JsonConvert.SerializeObject(user));
            editor.PutString("token", token);
            editor.Apply();
        }

        public DriverInfomation GetCurrentUser
        {
            get
            {
                DriverInfomation userInfo = new DriverInfomation();
                string user = preferences.GetString("userinfo", null);
                if (!string.IsNullOrEmpty(user))
                {
                    userInfo = JsonConvert.DeserializeObject<DriverInfomation>(user);
                    // userInfo.DriverId = Guid.Parse(userInfo.DriverId);
                }
                return userInfo;
            }
        }

        public string GetToken
        {
            get
            {
                return preferences.GetString("token", null);
            }
        }

    }
}