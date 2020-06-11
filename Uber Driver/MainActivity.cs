using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Support.V4.View;
using Com.Ittianyu.Bottomnavigationviewex;
using System;
using Uber_Driver.Adapter;
using Uber_Driver.Fragments;
using Android.Graphics;
using Android.Support.V4.App;
using Uber_Driver.EventListeners;
using Android.Gms.Maps.Model;
using Android.Support.V4.Content;
using Uber_Driver.DataModels;
using Android.Media;
using Uber_Driver.Helpers;
using Android.Content;
using Uber_Driver.Services;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Security;
using System.Net;
using LetsRide;
using LetsRide.DataModels;
using Uber_Driver.WebServices;
using LetsRide.Fragments;
using LetsRide.DataModels.Rides;

namespace Uber_Driver
{
    [Activity(Label = "@string/app_name", Theme = "@style/UberTheme", MainLauncher = false)]
    public class MainActivity : AppCompatActivity
    {
        HubConnection hubConnection;
        //Buttons
        Button goOnlineButton;

        //Views
        ViewPager viewpager;
        BottomNavigationViewEx bnve;

        //Fragments
        PreviousRideFragment previousRideFragment;
        HomeFragment homeFragment = new HomeFragment();
        RatingsFragment ratingsFragment = new RatingsFragment();
        EarningsFragment earningsFragment = new EarningsFragment();
        AccountFragment accountFragment = new AccountFragment();
        NewRequestFragment requestFoundDialogue;

        //PermissionRequest
        const int RequestID = 0;
        readonly string[] permissionsGroup =
        {
            Android.Manifest.Permission.AccessCoarseLocation,
            Android.Manifest.Permission.AccessFineLocation,
        };


        //EventListeners
        ProfileEventListener profileEventListener = new ProfileEventListener();
        AvailablityListener availablityListener;
        RideDetailsListener rideDetailsListener;
        NewTripEventListener newTripEventListener;


        //Map Stuffs
        Android.Locations.Location mLastLocation;
        LatLng mLastLatLng;
        Random random = new Random();

        //Flags
        bool availablityStatus;
        bool isBackground;
        bool newRideAssigned;
        string status = "NORMAL";
        bool isLocationAvailable = false;
        bool IsConnected = false;
        //REQUESTFOUND, ACCEPTED, ONTRIP

        //Datamodels
        RideDetails newRideDetails;
        RideRequest rideRequest;
        TripService tripService = new TripService();
        Rides rides;
        //MediaPlayer
        MediaPlayer player;
      
        //Helpers
        MapFunctionHelper mapHelper;

        Android.Support.V7.App.AlertDialog.Builder alert;
        Android.Support.V7.App.AlertDialog alertDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            //   ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
           // ShowProgressDialogue();
            ConnectViews();
            CheckSpecialPermission();
            InitializeConnection();
         //   CloseProgressDialogue();
           
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
        void PreviousRideAction()
        {
            previousRideFragment = new PreviousRideFragment(rides);
            previousRideFragment.Cancelable = false;
            var trans = SupportFragmentManager.BeginTransaction();
            previousRideFragment.Show(trans, "rideAction");
            previousRideFragment.Completed += PreviousRideFragment_Completed;
            previousRideFragment.Cancelled += PreviousRideFragment_Cancelled;
        }

        private async void PreviousRideFragment_Cancelled(object sender, PreviousRideFragment.EventParameter e)
        {
            if (previousRideFragment != null)
            {
                previousRideFragment.Dismiss();
            }
            ShowProgressDialogue();
            ActiveDrivers activeDrivers = new ActiveDrivers()
            {
                Action = "C",
                Reasons = e.Reason ,
                RideId= rides.RideId  
            };
            await new AvailabilityService().ActiveDriver(activeDrivers);
            CloseProgressDialogue();
            TakeDriverOnline();
        }

        private async void PreviousRideFragment_Completed(object sender, EventArgs e)
        {
            if(previousRideFragment!= null)
            {
                previousRideFragment.Dismiss();
            }
            ShowProgressDialogue();
            ActiveDrivers activeDrivers = new ActiveDrivers()
            {
                Action = "F",
                RideId = rides.RideId
            };
            await new AvailabilityService().ActiveDriver(activeDrivers);
            CloseProgressDialogue();
            TakeDriverOnline();
        }

       async void InitializeConnection()
        {
            try
            {
                
                hubConnection = new HubConnectionBuilder().WithUrl(LetsRideCredentials.HubUrl,
                    options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult(new AppData().GetToken);
                        //options.WebSocketConfiguration = sockets =>
                        //{
                        //    sockets.RemoteCertificateValidationCallback += new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });
                        //};
                    }).WithAutomaticReconnect().Build();
                hubConnection.Closed += HubConnection_Closed;
                hubConnection.Reconnected += HubConnection_Reconnected;
                hubConnection.Reconnecting += HubConnection_Reconnecting;
                hubConnection.On<bool, string>("OnConnected", (status, message) =>
                {
                    //CloseProgressDialogue();
                    if (status)
                    {
                        goOnlineButton.Enabled = true;
                        availablityStatus = true;
                        goOnlineButton.Text = "Go offline";
                        goOnlineButton.Background = ContextCompat.GetDrawable(this, Resource.Drawable.uberroundbutton_green);

                        //khkh
                        //  RideDetails rideDetail = JsonConvert.DeserializeObject<RideDetails>(message);
                        // RideAssigned(rideDetail);

                        //  newRideDetails = rideDetail;
                    }
                    else
                    {
                        availablityStatus = false;
                        goOnlineButton.Enabled = true;
                        goOnlineButton.Text = "Go Online";
                        //isConnectionSuccessful = false;
                        Android.Support.V7.App.AlertDialog.Builder alert1 = new Android.Support.V7.App.AlertDialog.Builder(this);
                        alert1.SetTitle("GO ONLINE");
                        alert1.SetMessage("Request Failed, " + message + ". Try Again");
                        alert1.SetPositiveButton("Continue", (senderAlert, args) =>
                        {
                            alert1.Dispose();
                        });

                        alert1.Show();
                    }

                });

                hubConnection.On<string>("ReceiveRequest", async (message) =>
                {
                    RideDetails rideDetail = JsonConvert.DeserializeObject<RideDetails>(message);
                    if (Guid.TryParse(rideDetail.RideId, out Guid rideid))
                    {
                        rideRequest = new RideRequest()
                        {
                            Action = "A",
                            RideId = rideid,
                            DriverId = new AppData().GetCurrentUser.DriverId,
                            RequestTime = DateTime.Now
                        };
                        RideAssigned(rideDetail);
                        await tripService.SaveRideRequest(rideRequest);
                    }


                });
                hubConnection.On<string>("RequestCancel", async (status) =>
                {
                   newRideAssigned = false;
                   availablityListener.OnRideRequestCancelOrTimeout(status);
                    if (status == "timeout")
                    {
                        rideRequest.RideStatus = "T";
                        rideRequest.Action = "E";
                         await tripService.SaveRideRequest(rideRequest);
                    }
                    else if (status == "cancel")
                    {
                        rideRequest.RideStatus = "C";
                        rideRequest.Action = "E";
                        await tripService.SaveRideRequest(rideRequest);
                    }
                });
                hubConnection.On<string>("ErrorInformation", (message) =>
                {
                    if (message == "notonline")
                    {
                    }
                    else
                    {
                        Android.Support.V7.App.AlertDialog.Builder alert1 = new Android.Support.V7.App.AlertDialog.Builder(this);
                        alert1.SetTitle("Request Failed");
                        alert1.SetMessage(message);
                        alert1.SetPositiveButton("Continue", (senderAlert, args) =>
                        {
                            alert1.Dispose();
                        });
                        alert1.Show();
                    }
                });
                earningsFragment.OnProgress += Earnings_OnProgress;
                earningsFragment.EndProgress += Earnings_EndProgress;
                accountFragment.EndProgress += EndProgress;
                accountFragment.OnProgress += OnProgress;
                
            }
            catch (Exception ex)
            {
                CloseProgressDialogue();
                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle("Error in connection");
                alert.SetMessage("Request Failed, " + ex.Message + ". Try Again");
                alert.SetPositiveButton("Continue", (senderAlert, args) =>
                {
                    alert.Dispose();
                });
                alert.Show();
            }

        }

        private void Earnings_EndProgress(object sender, EventArgs e)
        {
            CloseProgressDialogue();
        }

        private void Earnings_OnProgress(object sender, EventArgs e)
        {
            ShowProgressDialogue();
        }

        private void OnProgress(object sender, EventArgs e)
        {
            ShowProgressDialogue();
        }

        private void EndProgress(object sender, EventArgs e)
        {
            CloseProgressDialogue();
        }

        public override void OnBackPressed()
        {
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetTitle("Are you sure to Exit?");
            alert.SetMessage("You may lose your current processed data.");
            alert.SetPositiveButton("Yes", async (senderAlert, args) =>
            {

                alert.Dispose();
                if (IsConnected)
                {
                    await DisconnectAsync();
                }
                base.OnBackPressed();
            });
            alert.SetNegativeButton("No", (sendAlert, args) =>
            {
                alert.Dispose();
            });
            alert.Show();

            ///base.OnBackPressed();
        }
        private async Task HubConnection_Reconnecting(Exception arg)
        {
            //  ShowProgressDialogue();
            goOnlineButton.Text = "Reconnecting...";
            goOnlineButton.Enabled = false;

        }

        private async Task HubConnection_Reconnected(string arg)
        {
            goOnlineButton.Text = "Go Offline";
            goOnlineButton.Enabled = true;
            goOnlineButton.Background = ContextCompat.GetDrawable(this, Resource.Drawable.uberroundbutton_green);

            // CloseProgressDialogue();
        }

        private async Task HubConnection_Closed(Exception exception)
        {
            IsConnected = false;

            await Task.Delay(random.Next(1, 5) * 1000);
            try
            {
                goOnlineButton.Text = "Go Online";
                goOnlineButton.Enabled = true;
                goOnlineButton.Background = ContextCompat.GetDrawable(this, Resource.Drawable.uberroundbutton);
                availablityStatus = false;
                availablityListener = null;
                await ConnectAsync();

            }
            catch (Exception ex)
            {
                ExceptionDialogue("Error", ex.Message);

            }

            //  CloseProgressDialogue();


        }

        public async Task ConnectAsync()
        {
            if (IsConnected)
                return;
            await hubConnection.StartAsync();
            IsConnected = true;
        }

        public async Task DisconnectAsync()
        {
            if (!IsConnected)
                return;
            try
            {
                await hubConnection.DisposeAsync();
                IsConnected = false;
            }
            catch (Exception ex)
            {

                ExceptionDialogue("Error", ex.Message);
            }
        }

        void ConnectViews()
        {
            goOnlineButton = (Button)FindViewById(Resource.Id.goOnlineButton);
            bnve = (BottomNavigationViewEx)FindViewById(Resource.Id.bnve);
            bnve.EnableItemShiftingMode(false);
            bnve.EnableShiftingMode(false);

            goOnlineButton.Click += GoOnlineButton_Click;
            bnve.NavigationItemSelected += Bnve_NavigationItemSelected;


            var img0 = bnve.GetIconAt(0);
            var txt0 = bnve.GetLargeLabelAt(0);
            img0.SetColorFilter(Color.Rgb(24, 191, 242));
            txt0.SetTextColor(Color.Rgb(24, 191, 242));

            viewpager = (ViewPager)FindViewById(Resource.Id.viewpager);
            viewpager.OffscreenPageLimit = 3;
            viewpager.BeginFakeDrag();

            SetupViewPager();

            homeFragment.CurrentLocation += HomeFragment_CurrentLocation;
            homeFragment.TripActionArrived += HomeFragment_TripActionArrived;
            homeFragment.CallRider += HomeFragment_CallRider;
            homeFragment.Navigate += HomeFragment_Navigate;
            homeFragment.TripActionStartTrip += HomeFragment_TripActionStartTrip;
            homeFragment.TripActionEndTrip += HomeFragment_TripActionEndTrip;
        }

        async void HomeFragment_TripActionEndTrip(object sender, EventArgs e)
        {
            //Reset app
            try
            {
                if (!IsConnected)
                {
                    await ConnectAsync();
                }
                AcceptedDriver driverInfo = new AcceptedDriver()
                {
                    Status = "ended",
                    Latitude = mLastLatLng.Latitude,
                    Longitude = mLastLatLng.Longitude
                };

                string info = JsonConvert.SerializeObject(driverInfo);
                await hubConnection.InvokeAsync("TripUpdates", newRideDetails.CustomerId, newRideDetails.RideId, info);
                status = "NORMAL";
                homeFragment.ResetAfterTrip();
                ShowProgressDialogue();
                LatLng pickupLatLng = new LatLng(newRideDetails.PickupLat, newRideDetails.PickupLng);
                double fares = Convert.ToDouble(newRideDetails.TotalCost); // await mapHelper.CalculateFares(pickupLatLng, mLastLatLng);
                await newTripEventListener.UpdateStatus("E");
                CloseProgressDialogue();
                newTripEventListener = null;
                CollectPaymentFragment collectPaymentFragment = new CollectPaymentFragment(fares);
                collectPaymentFragment.Cancelable = false;
                var trans = SupportFragmentManager.BeginTransaction();
                collectPaymentFragment.Show(trans, "pay");
                collectPaymentFragment.PaymentCollected += (o, u) =>
                {
                    collectPaymentFragment.Dismiss();
                };
                ReActivate();
            }
            catch (Exception ex)
            {
                ExceptionDialogue("Error", ex.Message);
            }
        }

        public async void ReActivate()
        {

            try
            {
                ResponseData response = await availablityListener.ReActivate();
                if (!response.IsSuccess)
                {
                    ExceptionDialogue("Error", response.Message);
                }
                else
                {
                    if (!IsConnected)
                        await ConnectAsync();
                }
            }
            catch (Exception ex)
            {

                ExceptionDialogue("Error", ex.Message);
            }
        }

        void HomeFragment_TripActionStartTrip(object sender, EventArgs e)
        {
            Android.Support.V7.App.AlertDialog.Builder startTripAlert = new Android.Support.V7.App.AlertDialog.Builder(this);
            startTripAlert.SetTitle("START TRIP");
            startTripAlert.SetMessage("Are you sure");
            startTripAlert.SetPositiveButton("Continue", async (senderAlert, args) =>
            {
                try
                {
                    if (!IsConnected)
                    {
                        await ConnectAsync();
                    }
                    AcceptedDriver driverInfo = new AcceptedDriver()
                    {
                        Status = "ontrip",
                        Latitude = mLastLatLng.Latitude,
                        Longitude = mLastLatLng.Longitude
                    };

                    string info = JsonConvert.SerializeObject(driverInfo);
                    await hubConnection.InvokeAsync("TripUpdates", newRideDetails.CustomerId, newRideDetails.RideId, info);
                    status = "ONTRIP";
                    // Update Rider that Driver has started the trip
                    await newTripEventListener.UpdateStatus("S");
                }
                catch (Exception ex)
                {

                    ExceptionDialogue("Error", ex.Message);
                }
            });
            startTripAlert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                startTripAlert.Dispose();
            });
            startTripAlert.Show();
        }
        void HomeFragment_Navigate(object sender, EventArgs e)
        {
            string uriString = "";

            if (status == "ACCEPTED")
            {
                uriString = "google.navigation:q=" + newRideDetails.PickupLat.ToString() + "," + newRideDetails.PickupLng.ToString();
            }
            else
            {
                uriString = "google.navigation:q=" + newRideDetails.DestinationLat.ToString() + "," + newRideDetails.DestinationLng.ToString();
            }

            Android.Net.Uri googleMapIntentUri = Android.Net.Uri.Parse(uriString);
            Intent mapIntent = new Intent(Intent.ActionView, googleMapIntentUri);
            mapIntent.SetPackage("com.google.android.apps.maps");

            try
            {
                StartActivity(mapIntent);
            }
            catch
            {
                Toast.MakeText(this, "Google Map is not Installed on this device", ToastLength.Short).Show();
            }
        }
        void HomeFragment_CallRider(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("tel:" + newRideDetails.RiderPhone);
            Intent intent = new Intent(Intent.ActionDial, uri);
            StartActivity(intent);
        }
        async void HomeFragment_TripActionArrived(object sender, EventArgs e)
        {
            //Notifies Rider that Driver has arrived
            try
            {
                if (!IsConnected)
                {
                    await ConnectAsync();
                }
                AcceptedDriver driverInfo = new AcceptedDriver()
                {
                    Status = "arrived",
                    Latitude = mLastLatLng.Latitude,
                    Longitude = mLastLatLng.Longitude

                };

                string info = JsonConvert.SerializeObject(driverInfo);
                await hubConnection.InvokeAsync("TripUpdates", newRideDetails.CustomerId, newRideDetails.RideId, info);
                await newTripEventListener.UpdateStatus("A");
                status = "ARRIVED";
                LatLng pickupLatLng = new LatLng(newRideDetails.PickupLat, newRideDetails.PickupLng);
                LatLng destinationLatLng = new LatLng(newRideDetails.DestinationLat, newRideDetails.DestinationLng);

                ShowProgressDialogue();
                string directionJson = await mapHelper.GetDirectionJsonAsync(pickupLatLng, destinationLatLng);
                CloseProgressDialogue();

                //Clear Map
                homeFragment.mainMap.Clear();
                mapHelper.DrawTripToDestination(directionJson);
            }
            catch (Exception ex)
            {

                ExceptionDialogue("Error", ex.Message);
            }
        }
        async void HomeFragment_CurrentLocation(object sender, Helpers.LocationCallbackHelper.OnLocationCaptionEventArgs e)
        {
            mLastLocation = e.Location;
            mLastLatLng = new LatLng(mLastLocation.Latitude, mLastLocation.Longitude);


            if (availablityListener != null && isLocationAvailable)
            {
                await availablityListener.UpdateLocation(mLastLocation);
            }
            else if (!isLocationAvailable)
            {
                isLocationAvailable = true;
                TakeDriverOnline();
            }

            if (availablityStatus && availablityListener == null)
            {
                isLocationAvailable = true;
                TakeDriverOnline();
            }

            if (status == "ACCEPTED")
            {
                //Update and Animate driver movement to pick up lOcation
                LatLng pickupLatLng = new LatLng(newRideDetails.PickupLat, newRideDetails.PickupLng);
                mapHelper.UpdateMovement(mLastLatLng, pickupLatLng, "Rider");

                //Updates Location in rideRequest Table, so that Rider can receive Updates
                //  newTripEventListener.UpdateLocation(mLastLocation);
                UpdateLocation("accepted");

            }

            else if (status == "ARRIVED")
            {
                //newTripEventListener.UpdateLocation(mLastLocation);
                UpdateLocation("arrived");
            }

            else if (status == "ONTRIP")
            {
                //Update and animate driver movement to Destination
                LatLng destinationLatLng = new LatLng(newRideDetails.DestinationLat, newRideDetails.DestinationLng);
                mapHelper.UpdateMovement(mLastLatLng, destinationLatLng, "Destination");
                //Update Location on firebase
                //   newTripEventListener.UpdateLocation(mLastLocation);

            }
        }
        private async void UpdateLocation(string status)
        {
            try
            {
                if (!IsConnected)
                {
                    await ConnectAsync();
                }
                AcceptedDriver driverInfo = new AcceptedDriver()
                {
                    Status = status,
                    Latitude = mLastLatLng.Latitude,
                    Longitude = mLastLatLng.Longitude
                };
                string info = JsonConvert.SerializeObject(driverInfo);
                await hubConnection.SendAsync("TripUpdatesWithoutResponse", newRideDetails.CustomerId, newRideDetails.RideId, info);
            }
            catch (Exception ex)
            {

                ExceptionDialogue("Error", ex.Message);
            }
        }
        private async void TakeDriverOnline()
        {

            //  driverInfomation = new WebApiServices().GetDriverInformation();
            try
            {
                availablityListener = new AvailablityListener();
                ShowProgressDialogue();
                //  if (isLocationAvailable)
                ResponseData response = await availablityListener.Create(mLastLocation, hubConnection);

                if (response.IsSuccess)
                {
                    goOnlineButton.Enabled = false;
                    await ConnectAsync();
                    CloseProgressDialogue();
                    availablityListener.RideAssigned += AvailablityListener_RideAssigned;
                    availablityListener.RideTimedOut += AvailablityListener_RideTimedOut;
                    availablityListener.RideCancelled += AvailablityListener_RideCancelled;
                }
                else
                {
                    CloseProgressDialogue();
                    if (!string.IsNullOrEmpty(response.RecordsInString))
                    {
                        rides = JsonConvert.DeserializeObject<Rides>(response.RecordsInString);
                        PreviousRideAction();
                    }
                    else
                    {
                        goOnlineButton.Enabled = true;
                        goOnlineButton.Text = "Go Online";
                        isLocationAvailable = false;
                        Android.Support.V7.App.AlertDialog.Builder alert1 = new Android.Support.V7.App.AlertDialog.Builder(this);
                        alert1.SetTitle("Error in connection");
                        alert1.SetMessage("Request Failed, " + response.Message + ". Try Again");
                        alert1.SetPositiveButton("Continue", (senderAlert, args) =>
                        {
                            alert1.Dispose();
                        });
                        alert1.Show();

                    }
                }

            }
            catch (Exception ex)
            {
                goOnlineButton.Enabled = true;
                goOnlineButton.Text = "Go Online";
                isLocationAvailable = false;
                ExceptionDialogue("Error", ex.Message);
            }
        }
        async void TakeDriverOffline()
        {
            homeFragment.GoOffline();
            await DisconnectAsync();
        }
        void AvailablityListener_RideAssigned(object sender, AvailablityListener.RideAssignedIDEventArgs e)
        {

            //Get Details
            rideDetailsListener = new RideDetailsListener();
            rideDetailsListener.Create(newRideDetails);
            rideDetailsListener.RideDetailsFound += RideDetailsListener_RideDetailsFound;
            rideDetailsListener.RideDetailsNotFound += RideDetailsListener_RideDetailsNotFound;
        }
        void RideAssigned(RideDetails details)
        {
            rideDetailsListener = new RideDetailsListener();
            rideDetailsListener.RideDetailsFound += RideDetailsListener_RideDetailsFound;
            rideDetailsListener.RideDetailsNotFound += RideDetailsListener_RideDetailsNotFound;
            rideDetailsListener.Create(details);
        }
        void RideDetailsListener_RideDetailsNotFound(object sender, EventArgs e)
        {



        }
        void CreateNewRequestDialogue()
        {
            requestFoundDialogue = new NewRequestFragment(newRideDetails);
            requestFoundDialogue.Cancelable = false;
            var trans = SupportFragmentManager.BeginTransaction();
            requestFoundDialogue.Show(trans, "Request");

            //Play Alert
            player = MediaPlayer.Create(this, Resource.Raw.alert);
            player.Start();

            requestFoundDialogue.RideRejected += RequestFoundDialogue_RideRejected;
            requestFoundDialogue.RideAccepted += RequestFoundDialogue_RideAccepted;
        }
        async void RequestFoundDialogue_RideAccepted(object sender, EventArgs e)
        {
            try
            {
                //Stop Alert 
                if (player != null)
                {
                    player.Stop();
                    player = null;
                }

                //Dissmiss Dialogue
                if (requestFoundDialogue != null)
                {
                    requestFoundDialogue.Dismiss();
                    requestFoundDialogue = null;
                }
                status = "ACCEPTED";
                newTripEventListener = new NewTripEventListener(newRideDetails.RideId);
                ShowProgressDialogue();
                if (!IsConnected)
                {
                    await hubConnection.StartAsync();
                }
                rideRequest.RideStatus = "A";
                rideRequest.Action = "E";
                await hubConnection.InvokeAsync("RequestResponse", true, newRideDetails.CustomerId, newRideDetails.RideId, new AppData().GetCurrentUser.DriverId.ToString(), mLastLocation.Latitude, mLastLocation.Longitude);
                await tripService.SaveRideRequest(rideRequest);
                homeFragment.CreateTrip(newRideDetails.RiderName);
                mapHelper = new MapFunctionHelper(Resources.GetString(Resource.String.mapkey), homeFragment.mainMap);
                LatLng pickupLatLng = new LatLng(newRideDetails.PickupLat, newRideDetails.PickupLng);
                string directionJson = await mapHelper.GetDirectionJsonAsync(mLastLatLng, pickupLatLng);
                CloseProgressDialogue();
                mapHelper.DrawTripOnMap(directionJson);
            }
            catch (Exception ex)
            {

                ExceptionDialogue("Error in connection", ex.Message);
                status = "NORMAL";
            }

        }
        void ExceptionDialogue(string title, string message)
        {
            Android.Support.V7.App.AlertDialog.Builder alert1 = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert1.SetTitle(title);
            alert1.SetMessage(message);
            alert1.SetPositiveButton("Continue", (senderAlert, args) =>
            {
                alert1.Dispose();
            });
            alert1.Show();
        }
        async void RequestFoundDialogue_RideRejected(object sender, EventArgs e)
        {
            try
            {

                ////Stop Alert
                if (player != null)
                {
                    player.Stop();
                    player = null;
                }
                //Dissmiss Dialogue
                if (requestFoundDialogue != null)
                {
                    requestFoundDialogue.Dismiss();
                    requestFoundDialogue = null;
                }
                //  newRideDetails = new RideDetails();
                // availablityListener.ReActivate();

                //Do other stuff


                if (!IsConnected)
                    await ConnectAsync();

                await hubConnection.InvokeAsync("RequestResponse", false, newRideDetails.CustomerId,
                    newRideDetails.RideId, new AppData().GetCurrentUser.DriverId.ToString(), null, null);
                rideRequest.RideStatus = "R";
                rideRequest.Action = "E";
                await tripService.SaveRideRequest(rideRequest);

            }
            catch (Exception ex)
            {

                Android.Support.V7.App.AlertDialog.Builder alert1 = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert1.SetTitle("Error in connection");
                alert1.SetMessage("Request Failed, " + ex.Message + ". Try Again");
                alert1.SetPositiveButton("Continue", (senderAlert, args) =>
                {
                    alert1.Dispose();
                });
                alert1.Show();
            }
        }

        void RideDetailsListener_RideDetailsFound(object sender, RideDetailsListener.RideDetailsEventArgs e)
        {
            if (status != "NORMAL")
            {
                return;
            }
            newRideDetails = e.RideDetails;

            if (!isBackground)
            {
                CreateNewRequestDialogue();
            }
            else
            {
                newRideAssigned = true;
                NotificationHelper notificationHelper = new NotificationHelper();
                if ((int)Build.VERSION.SdkInt >= 26)
                {
                    notificationHelper.NotifyVersion26(this, Resources, (NotificationManager)GetSystemService(NotificationService));
                }
            }
        }
        void AvailablityListener_RideTimedOut(object sender, EventArgs e)
        {
            if (requestFoundDialogue != null)
            {
                requestFoundDialogue.Dismiss();
                requestFoundDialogue = null;
                player.Stop();
                player = null;
            }

            Toast.MakeText(this, "New trip Timeout", ToastLength.Short).Show();
            //ResponseData response =  await availablityListener.ReActivate();
            // if (!response.IsSuccess)
            // {
            //     ExceptionDialogue("Error", response.Message);
            // }
        }
        void AvailablityListener_RideCancelled(object sender, EventArgs e)
        {
            if (requestFoundDialogue != null)
            {
                requestFoundDialogue.Dismiss();
                requestFoundDialogue = null;
                player.Stop();
                player = null;
            }
            Toast.MakeText(this, "New trip was cancelled", ToastLength.Short).Show();
            // availablityListener.ReActivate();
        }

        void GoOnlineButton_Click(object sender, EventArgs e)
        {
            if (!CheckSpecialPermission())
            {
                return;
            }

            if (availablityStatus)
            {
                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle("GO OFFLINE");
                alert.SetMessage("You will not be able to receive Ride Request");
                alert.SetPositiveButton("Continue", (senderAlert, args) =>
                {
                    TakeDriverOffline();
                });

                alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    alert.Dispose();
                });

                alert.Show();
            }
            else
            {
                //  ShowProgressDialogue();
                // availablityStatus = true;
                goOnlineButton.Enabled = false;
                goOnlineButton.Text = "Connecting...";
                homeFragment.GoOnline();

            }

        }
        private void Bnve_NavigationItemSelected(object sender, Android.Support.Design.Widget.BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            if (e.Item.ItemId == Resource.Id.action_earning)
            {
                viewpager.SetCurrentItem(1, true);
                BnveToAccentColor(1);
            }
            else if (e.Item.ItemId == Resource.Id.action_home)
            {
                viewpager.SetCurrentItem(0, true);
                BnveToAccentColor(0);
            }
            //else if (e.Item.ItemId == Resource.Id.action_rating)
            //{
            //    viewpager.SetCurrentItem(2, true);
            //    BnveToAccentColor(2);

            //}
            else if (e.Item.ItemId == Resource.Id.action_account)
            {
                viewpager.SetCurrentItem(2, true);
                BnveToAccentColor(2);
            }

        }
        void BnveToAccentColor(int index)
        {
            //Set all to white
            var img = bnve.GetIconAt(1);
            var txt = bnve.GetLargeLabelAt(1);
            img.SetColorFilter(Color.Rgb(255, 255, 255));
            txt.SetTextColor(Color.Rgb(255, 255, 255));

            var img0 = bnve.GetIconAt(0);
            var txt0 = bnve.GetLargeLabelAt(0);
            img0.SetColorFilter(Color.Rgb(255, 255, 255));
            txt0.SetTextColor(Color.Rgb(255, 255, 255));

            var img2 = bnve.GetIconAt(2);
            var txt2 = bnve.GetLargeLabelAt(2);
            img2.SetColorFilter(Color.Rgb(255, 255, 255));
            txt2.SetTextColor(Color.Rgb(255, 255, 255));

            //var img3 = bnve.GetIconAt(3);
            //var txt3 = bnve.GetLargeLabelAt(3);
            //img2.SetColorFilter(Color.Rgb(255, 255, 255));
            //txt2.SetTextColor(Color.Rgb(255, 255, 255));

            //Sets Accent Color
            var imgindex = bnve.GetIconAt(index);
            var textindex = bnve.GetLargeLabelAt(index);
            imgindex.SetColorFilter(Color.Rgb(24, 191, 242));
            textindex.SetTextColor(Color.Rgb(24, 191, 242));

        }
        private void SetupViewPager()
        {
            ViewPagerAdapter adapter = new ViewPagerAdapter(SupportFragmentManager);
            adapter.AddFragment(homeFragment, "Home");
            adapter.AddFragment(earningsFragment, "History");
           // adapter.AddFragment(ratingsFragment, "Rating");
            adapter.AddFragment(accountFragment, "Account");
            viewpager.Adapter = adapter;
        }
        bool CheckSpecialPermission()
        {
            bool permissionGranted = false;
            if (ActivityCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                RequestPermissions(permissionsGroup, RequestID);
            }
            else
            {
                permissionGranted = true;
            }

            return permissionGranted;
        }
        protected override void OnPause()
        {
            isBackground = true;
            base.OnPause();
        }

        protected override async void OnResume()
        {
            isBackground = false;


            if (newRideAssigned)
            {
                if (!IsConnected)
                    await ConnectAsync();
                CreateNewRequestDialogue();
                newRideAssigned = false;
            }
            base.OnResume();
        }
    }
}