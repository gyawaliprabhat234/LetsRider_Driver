using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using Java.Util;
using LetsRide;
using Newtonsoft.Json;
using Uber_Driver.DataModels;
using Uber_Driver.EventListeners;
using Uber_Driver.Helpers;
using Uber_Driver.Services;
using Uber_Driver.WebServices;

namespace Uber_Driver.Activities
{
    [Activity(Label = "RegistrationActivity", MainLauncher = false, Theme = "@style/UberTheme")]
    public class RegistrationActivity : AppCompatActivity
    {
        TextInputLayout fullNameText;
        TextInputLayout phoneText;
        TextInputLayout emailText;
        TextInputLayout passwordText;
        Button registerButton;
        CoordinatorLayout rootView;
        FirebaseDatabase database;
        FirebaseAuth mAuth;
        FirebaseUser currentUser;

        TaskCompletionListeners taskCompletionListener = new TaskCompletionListeners();

        Android.Support.V7.App.AlertDialog.Builder alert;
        Android.Support.V7.App.AlertDialog alertDialog;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.register);
            ConnectViews();
          //  SetupFireBase();
        }

        void ShowProgressDialogue()
        {
            alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetView(Resource.Layout.progress);
            alert.SetCancelable(false);
            alertDialog = alert.Show();
        }

        void CloseProgressDialogue()
        {
            if (alert != null)
            {
                alertDialog.Dismiss();
                alertDialog = null;
                alert = null;
            }
        }

     
        void ConnectViews()
        {
            fullNameText = (TextInputLayout)FindViewById(Resource.Id.fullNameText);
            phoneText = (TextInputLayout)FindViewById(Resource.Id.phoneText);
            emailText = (TextInputLayout)FindViewById(Resource.Id.emailText);
            passwordText = (TextInputLayout)FindViewById(Resource.Id.passwordText);
            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            registerButton = (Button)FindViewById(Resource.Id.registerButton);

            registerButton.Click += RegisterButton_Click;
        }

        private async void RegisterButton_Click(object sender, EventArgs e)
        {
            DriverInfomation driverInfomation = new DriverInfomation();
            string fullname, phone, email, password;

            fullname = fullNameText.EditText.Text;
            phone = phoneText.EditText.Text;
            email = emailText.EditText.Text;
            password = passwordText.EditText.Text;
          
            if (phone.Length != 10)
            {
                Snackbar.Make(rootView, "Please Enter Valid Phone Number", Snackbar.LengthShort).Show();
                return;

            }
            else if (password.Length < 8)
            {
                Snackbar.Make(rootView, "Please Enter a Valid Password", Snackbar.LengthShort).Show();
                return;
            }
            if (Decimal.TryParse(phone, out decimal result))
            {
                driverInfomation.DriverPhoneNumber = result;
                driverInfomation.Password = password;
            }
            else
            {
                Snackbar.Make(rootView, "Please Enter Valid Phone Number", Snackbar.LengthShort).Show();
                return;
            }

            ShowProgressDialogue();
            ResponseData response = await new LoginService().CheckDriverExist(driverInfomation);
            CloseProgressDialogue();
            if (response.IsSuccess)
            {
                driverInfomation.DriverId = response.Id;
                Driver_CodeSent(driverInfomation);
            }
            else
            {
                Snackbar.Make(rootView, response.Message, Snackbar.LengthShort).Show();
            }
           

        }

        private void Driver_CodeSent(DriverInfomation info)
        {
            string driverInfo = JsonConvert.SerializeObject(info);
            var activity = new Intent(this, typeof(OtpCheckingActivity));
            activity.PutExtra("driver", driverInfo);
            this.Finish();
            StartActivity(activity);
        }


    }
}