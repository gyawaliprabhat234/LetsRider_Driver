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
using LetsRide;
using Newtonsoft.Json;
using Uber_Driver.DataModels;
using Uber_Driver.EventListeners;
using Uber_Driver.Helpers;
using Uber_Driver.WebServices;

namespace Uber_Driver.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/UberTheme", MainLauncher = false)]
    public class LoginActivity : AppCompatActivity
    {
        Button loginButton;
        TextInputLayout textInputMobile;
        TextInputLayout textInputPassword;
        CoordinatorLayout rootView;
        TextView clickToSignUp;

        Android.Support.V7.App.AlertDialog.Builder alert;
        Android.Support.V7.App.AlertDialog alertDialog;
        DriverInfomation driverInfo;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.login);
            ConnectViews();
            string driver = Intent.GetStringExtra("driver") ?? string.Empty;
            driverInfo = string.IsNullOrEmpty(driver)? new DriverInfomation() : JsonConvert.DeserializeObject<DriverInfomation>(driver);
            textInputMobile.EditText.Text = driverInfo.DriverPhoneNumber != 0 ? String.Format("{0:0}", driverInfo.DriverPhoneNumber) : "";
            textInputPassword.EditText.Text = driverInfo.Password;
        }

    

        void ConnectViews()
        {
            loginButton = (Button)FindViewById(Resource.Id.loginButton);
            textInputMobile = (TextInputLayout)FindViewById(Resource.Id.phoneText);
            textInputPassword = (TextInputLayout)FindViewById(Resource.Id.passwordText);
            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            clickToSignUp = (TextView)FindViewById(Resource.Id.clickToSignUpText);
           
            loginButton.Click += LoginButton_Click;
            clickToSignUp.Click += ClickToSignUp_Click;
        }

        private void ClickToSignUp_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(RegistrationActivity));
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {

            string phone, password;
            phone = textInputMobile.EditText.Text;
            password = textInputPassword.EditText.Text;
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
                driverInfo.DriverPhoneNumber = result;
                driverInfo.Password = password;
            }
            else
            {
                Snackbar.Make(rootView, "Please Enter Valid Phone Number", Snackbar.LengthShort).Show();
                return;
            }

            ShowProgressDialogue();
            ResponseData response = await new LoginService().CheckDriverLoginInfo(driverInfo);
            CloseProgressDialogue();
            if (response.IsSuccess && !string.IsNullOrEmpty(response.Token))
            {
                driverInfo = JsonConvert.DeserializeObject<DriverInfomation>(response.RecordsInString);
                driverInfo.Password = password;
                new AppData().SetUserInformation(driverInfo, response.Token);
                Login_Successful();
            }
            else
            {
                Snackbar.Make(rootView, response.Message, Snackbar.LengthShort).Show();
            }
        }

        private void Login_Successful()
        {
            
            var activity = new Intent(this, typeof(MainActivity));
           // activity.PutExtra("IsActive", false);
            this.Finish();
            StartActivity(activity);
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
           if(alert != null)
            {
                alertDialog.Dismiss();
                alertDialog = null;
                alert = null;
            }
        }
       
    }
}