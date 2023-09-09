using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicDoor.Droid
{
    [Service]
    internal class ForgroundServiceSMS : Service
    {
        private const string CHANNEL_ID = "Test";
        private const int NOTIFICATION_ID = 1;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Info("Custom LOG ", "SMS Service detected");

            // Create a notification channel (required for Android 8.0 and higher)
            CreateNotificationChannel();
            NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this, CHANNEL_ID).SetContentTitle("Magic Door Service").SetContentText("Service is running in the background").SetSmallIcon(Resource.Drawable.ic_notification_icon);

            Notification notification = notificationBuilder.Build();

            StartForeground(NOTIFICATION_ID, notification);


            // Return StartCommandResult.Sticky if you want the service to be restarted if it's killed by the system
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            // Clean up resources and stop the service
            base.OnDestroy();
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelName = "Magic Door";
                var channelDescription = "Background service for ";
                var channel = new NotificationChannel(CHANNEL_ID, channelName, NotificationImportance.Default)
                {
                    Description = channelDescription
                };

                var notificationManager = GetSystemService(NotificationService) as NotificationManager;
                notificationManager.CreateNotificationChannel(channel);
            }
        }

    }
}