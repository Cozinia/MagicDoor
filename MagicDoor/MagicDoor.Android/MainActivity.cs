using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Content;
using Android.Provider;
using Android.Support.V4.App;
using Android.Nfc;
using Android.Util;
using AndroidX.Core.App;
using Google.Android.Material.Snackbar;
using Android;
using Xamarin.Forms;
using Xamarin.Essentials;
using PP = Plugin.Permissions;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;
using Android.Widget;

namespace MagicDoor.Droid
{
    [Activity(Label = "Magic Door", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation  | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        private SmsReceiver smsReceiver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            SmsReceiver receiver = new SmsReceiver();


            var smsFilter = new
            IntentFilter("android.provider.Telephony.SMS_RECEIVED")
            {
                Priority =
            (int)IntentFilterPriority.HighPriority
            };
            RegisterReceiver(receiver, smsFilter);
            RequestPermissions();

            var serviceIntent = new Intent(this, typeof(ForgroundServiceSMS));
            StartService(serviceIntent);


            LoadApplication(new App());



        }


        async void RequestPermissions()
        {
            try
            {
                var status = await PP.CrossPermissions.Current.CheckPermissionStatusAsync<PP.SmsPermission>();
                if (status != PP.Abstractions.PermissionStatus.Granted)
                {
                    if (await PP.CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(PP.Abstractions.Permission.Sms))
                    {
                        //Gunna need that location
                    }

                    status = await PP.CrossPermissions.Current.RequestPermissionAsync<PP.SmsPermission>();
                }

                if (status == PP.Abstractions.PermissionStatus.Granted)
                {
                    //Query permission
                }
                else if (status != PP.Abstractions.PermissionStatus.Unknown)
                {
                    //location denied
                }
            }
            catch (Exception ex)
            {
                //Something went wrong
                Toast.MakeText(BaseContext, $"Exception: {ex}", ToastLength.Short).Show();
            }

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnDestroy()
        {
            // Unregister the BroadcastReceiver when the app is destroyed
            if (smsReceiver != null)
            {
                UnregisterReceiver(smsReceiver);
                smsReceiver = null;
            }

            base.OnDestroy();
        }

    }
}