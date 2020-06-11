using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using LetsRide;
using Newtonsoft.Json;
using Uber_Driver.DataModels;
using Uber_Driver.WebServices;

namespace Uber_Driver.Activities
{
    [Activity(Label = "@string/app_name")]
    public class OtpCheckingActivity : Activity
    {
      
        TextInputLayout otpcodeText;
        TextView resendHelperText;
        Button checkOtpButton;
        Button resendText;
        CoordinatorLayout rootView;
        Android.Support.V7.App.AlertDialog.Builder alert;
        Android.Support.V7.App.AlertDialog alertDialog;
        DriverInfomation driverInfo;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.otp_check);
            otpcodeText = (TextInputLayout)FindViewById(Resource.Id.otpcodeText);
            checkOtpButton = (Button)FindViewById(Resource.Id.checkOtpButton);
            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            resendHelperText = (TextView)FindViewById(Resource.Id.resendHelperText);
            resendText = (Button)FindViewById(Resource.Id.resendText);
            ResendCodeDelay();
            resendText.Click += ResendText_Click;
            checkOtpButton.Click += CheckOtpButton_Click;
            string driver = Intent.GetStringExtra("driver") ?? string.Empty;
            driverInfo = JsonConvert.DeserializeObject<DriverInfomation>(driver);

        }

        private async void ResendText_Click(object sender, EventArgs e)
        {
            ShowProgressDialogue();
            Guid d = driverInfo.DriverId;
            ResponseData response = await new LoginService().ResendVerificationCode(driverInfo);
            CloseProgressDialogue();
            if (response.IsSuccess)
            {
                ResendCodeDelay();
                Snackbar.Make(rootView, "Successfully code sent...", Snackbar.LengthShort).Show();
            }
            else
            {
                Snackbar.Make(rootView, response.Message, Snackbar.LengthShort).Show();
            }
        }

        public async void ResendCodeDelay()
        {
            resendText.Visibility = ViewStates.Gone;
            resendHelperText.Visibility = ViewStates.Visible;
            for (int i = 29; i > 1; i--)
            {
                resendHelperText.Text = "Please wait " + i.ToString() + " seconds to resend code";
                await Task.Delay(1000);
            }
            resendHelperText.Text = "Now wait is finished. You can resend code again";
            await Task.Delay(1000);
            resendText.Visibility = ViewStates.Visible;
            resendHelperText.Visibility = ViewStates.Gone;


        }

        private async void CheckOtpButton_Click(object sender, EventArgs e)
        {

            if (otpcodeText.EditText.Text.Length != 6)
            {
                Snackbar.Make(rootView, "Code is not valid", Snackbar.LengthShort).Show();
                return;
            }
            if (Decimal.TryParse(otpcodeText.EditText.Text.Trim(), out decimal result))
            {
                driverInfo.OTP = result;
            }
            else
            {
                Snackbar.Make(rootView, "Code is not valid", Snackbar.LengthShort).Show();
                return;
            }

            ShowProgressDialogue();
            ResponseData response = await new LoginService().CheckOtpCodeForDriver(driverInfo);
            CloseProgressDialogue();
            if (response.IsSuccess && response.IsValid)
            {

                Login_Successful(driverInfo);

            }
            else
            {
                Snackbar.Make(rootView, response.Message, Snackbar.LengthShort).Show();
            }
        }

        private void Login_Successful(DriverInfomation user)
        {
            string driverInfo = JsonConvert.SerializeObject(user);        
            var activity = new Intent(this, typeof(LoginActivity));
            activity.PutExtra("driver", driverInfo);
            this.Finish();
            StartActivity(activity);
        }


        void ShowProgressDialogue()
        {
            if (alert == null)
            {
                alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetView(Resource.Layout.progress);
                alert.SetCancelable(false);
                alertDialog = alert.Show();
            }
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
    }
}