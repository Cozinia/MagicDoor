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
using Android.Util;
using Android.Telephony;


namespace MagicDoor.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = false, Label = "SMS Receiver")]
    [IntentFilter(new string[] { "android.provider.Telephony.SMS_RECEIVED" }, Priority = Int32.MaxValue)]
    public class SmsReceiver : BroadcastReceiver
    {
        public SmsReceiver():base()
            {

            }

        public static readonly string INTENT_ACTION = "android.provider.Telephony.SMS_RECEIVED";

        public override void OnReceive(Context context, Intent intent)
        {

            if (intent.Action == INTENT_ACTION)
            {
                Log.Info("Custom LOG ", "SMS detected, app in backround");

               // Get the SMS messages from the intent
                var bundle = intent.Extras;
                if (bundle != null)
                {
                    #pragma warning disable CS0618 // Type or member is obsolete
                    var pdus = bundle.Get("pdus");
                    #pragma warning restore CS0618 // Type or member is obsolete
                    if (pdus != null)
                    {
                        var pdusArray = (Java.Lang.Object[])pdus;
                        var smsMessages = new SmsMessage[pdusArray.Length];

                        // Convert the PDUs to SmsMessage objects
                        for (int i = 0; i < pdusArray.Length; i++)
                        {
                            #pragma warning disable CS0618 // Type or member is obsolete
                            smsMessages[i] = SmsMessage.CreateFromPdu((byte[])pdusArray[i]);
                            #pragma warning restore CS0618 // Type or member is obsolete
                        }

                        // Process each SMS message
                        foreach (var smsMessage in smsMessages)
                        {
                            string senderPhoneNumber = smsMessage.OriginatingAddress;
                            string messageBody = smsMessage.MessageBody;
                            bool contactInDb = ContactService.CheckIfNumberExists(senderPhoneNumber);

                            if (contactInDb)
                            {
                                Toast.MakeText(context, $"SMS from {senderPhoneNumber}: {messageBody}", ToastLength.Long).Show();
                            }
                            else
                            {
                                Toast.MakeText(context, "Contact not in DB.", ToastLength.Long).Show();
                            }
                           
                        }
                    }
                }

            }
        }
    }
}