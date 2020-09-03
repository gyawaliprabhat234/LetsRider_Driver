using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Uber_Driver.DataModels;
using Uber_Driver.Helpers;
using Uber_Driver.WebServices;

namespace Uber_Driver.Activities
{
    [Activity(Label = "@string/app_name", Theme ="@style/MyTheme.Splash",NoHistory =true, MainLauncher = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
        }

        protected async override void OnResume()
        {
            base.OnResume();
        //    ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;
            //   FirebaseUser currentUser = AppDataHelper.GetCurrentUser();
            if ((await new LoginService().IsDriverLoggedIn()).IsSuccess)
            {
                StartActivity(typeof(MainActivity));
            }
            else
            {
                StartActivity(typeof(LoginActivity));
            }
        }
    }
}