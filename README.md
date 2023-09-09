
# Magic Door

This application has been exclusively developed for the Android operating system, utilizing the Xamarin Forms framework.



## About the app

An automated message will be dispatched to the user's mobile device. The application is programmed to promptly detect incoming SMS notifications. If the originating phone number align with an entry within the  local database, the system will initiate a predefined action, such as facilitating the remote unlocking of a designated access point, ensuring seamless access control and operational efficiency.

## How's working?
This application has been meticulously designed to maintain full functionality even when running in the background or when closed. This seamless operation is made possible through the implementation of a foreground service, which actively generates user notifications to signal the app's continued presence and operation in the background. To ensure uninterrupted service, it is imperative that users grant the necessary permissions for SMS message interception, thereby enabling the application to provide its intended functionality.







## About the code

### Database

#### Creating the database

Using the `SQLite` library I've created the a local database using the `CreateTableAsync` method.

```
 public static async Task Init()
        {
            if (db != null)
            {
                return;
            }
            // Get an absolute path to the database file
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "MagicDoorDB.db");

            db = new SQLiteAsyncConnection(databasePath);
           await db.CreateTableAsync<Contacts>();
                      
        }
```
#### Adding contacts

In the interest of regional compatibility and effective functionality, a `+4` prefix has been applied to the phone number configuration within the application. However, it is imperative to modify this prefix to correspond with the appropriate regional prefix in their respective location, ensuring seamless and accurate communication alignment with local standards.

```
 public static async Task addContact(string _name, string _phone)
        {
            await Init();
            var contact = new Contacts()
            {
                Name = _name,
                Phone = "+4" + _phone
            };
            await db.InsertAsync(contact);
        }
```

#### Removing contacts

To remove contacts from the database, a streamlined process has been established. This process involves the transmission of a unique Contact ID, which is subsequently utilized in conjunction with the `DeleteAsync` method from the `SQLite` library. This method ensures the precise and secure removal of the designated contact, maintaining the integrity and efficiency of the contact management system.

```

        public static async Task removeContact(int id)
        {
            try
            {
                await Init();
                await db.DeleteAsync<Contacts>(id);
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error deleting contact: {ex.Message}");
            }
        }

```

#### Get the contacts

To retrieve the contacts, the method involves querying the entirety of the database contents.

```
        public static async Task<IEnumerable<Contacts>> getContacts()
        {
            await Init();

            var _table_contacts = await db.Table<Contacts>().ToListAsync();
            return _table_contacts;
        }
```
#### Verify if a number is in the database

The `CheckIfNumberExists` method is responsible for checking if a contact with a specific phone number exists in the SQLite database. It establishes a connection to the database, queries the Contacts table for a matching phone number, and returns true if a match is found and false otherwise. The use of a using statement ensures proper resource management for the database connection.

```
 public static bool CheckIfNumberExists(string phoneNumber)
        {
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "MagicDoorDB.db");
            using (SQLiteConnection connection = new SQLiteConnection(databasePath))
            {
                var existingNumber = connection.Table<Contacts>()
                    .FirstOrDefault(x => x.Phone == phoneNumber);

                return existingNumber != null;
            }
        }

```

### Intercept SMS messages

#### Request permissions

To facilitate the interception of SMS messages, users must grant permission for this specific functionality. This requisite can be fulfilled by implementing a `RequestPermissions` method within the `MainActivity.cs` file.The code is enclosed within a try-catch block to proactively handle potential errors that may arise during its execution.

```
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
```

The code below is declared in the `OnCreate` method of the `MainActivity` class and sets up the necessary infrastructure to intercept incoming SMS messages. It defines an IntentFilter that listens for incoming SMS events with high priority, registers a BroadcastReceiver to handle these events, and requests the necessary permissions from the user to read SMS messages. This setup allows the app to read incoming SMS messages and perform specific actions when SMS messages are received.

```

var smsFilter = new IntentFilter("android.provider.Telephony.SMS_RECEIVED")
{
    Priority = (int)IntentFilterPriority.HighPriority
};
RegisterReceiver(receiver, smsFilter);

```

#### Setting the XAML tags

In Xamarin.Forms, the tags provided below are XML elements used in the `AndroidManifest.xml` file. These elements are used to declare permissions required by the Android operating system for your app to access certain device features or resources.

```
  <uses-permission android:name="android.permission.RECEIVE_SMS" />
```

* This permission allows the Android app to receive incoming SMS messages on the user's device. It's necessary for intercepting or listening to incoming SMS messages.

```
  <uses-permission android:name="android.permission.READ_SMS" />
```

* This permission grants the Android app permission to read SMS messages stored on the device. It's used when the app needs to access and read SMS messages that have already been received and stored on the device.

#### Broadcast class

To facilitate the interception of SMS messages, it is imperative to create a class derived from the Broadcast class, enabling access to the full array of methods implemented within this class.

#### Defining a BroadcastReceiver

```
[BroadcastReceiver(Enabled = true, Exported = false, Label = "SMS Receiver")]
```

* `Enabled = true` This attribute indicates that the BroadcastReceiver is enabled and capable of receiving broadcasts.
* `Exported = false` This attribute specifies that the BroadcastReceiver is not intended to be accessible by other apps or components outside of its own app. It restricts external access.
* `Label = "SMS Receiver"` This is just a human-readable label assigned to the BroadcastReceiver for identification purposes.

```
[IntentFilter(new string[] { "android.provider.Telephony.SMS_RECEIVED" }, Priority = Int32.MaxValue)]
```

* `[IntentFilter]` This attribute defines an IntentFilter, which specifies the type of broadcast events that the BroadcastReceiver should listen for.
* `new string[] { "android.provider.Telephony.SMS_RECEIVED" }` This part of the IntentFilter specifies the action or event that the BroadcastReceiver should respond to. In this case, it listens for the `android.provider.Telephony.SMS_RECEIVED` action, which indicates the reception of SMS messages.
* `Priority = Int32.MaxValue` This attribute sets the priority of the BroadcastReceiver for handling incoming broadcasts. By setting it to `Int32.MaxValue`, this is giving this BroadcastReceiver the highest possible priority, meaning it will be one of the first components to respond to the `SMS_RECEIVED` event.

#### OnReceive method

The `OnReceive` method listens for incoming SMS messages, extracts their content and sender's phone number, checks if the sender's number is in the database, and displays toast notifications accordingly.

```

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

```

Explaining the code:

* `public override void OnReceive(Context context, Intent intent)` This method is an override of the OnReceive method for a BroadcastReceiver. It's called when a broadcast event occurs, in this case, when an SMS message is received.

* `if (intent.Action == INTENT_ACTION)` This condition checks if the action associated with the received intent matches a predefined constant called `INTENT_ACTION`. This is done to ensure that the code responds to SMS-related events specifically.

* `var bundle = intent.Extras` This line extracts the additional data bundled with the incoming SMS intent.

* `var pdus = bundle.Get("pdus")` This line retrieves the Protocol Data Units(PDU) from the bundle. PDUs represent the raw binary data of SMS messages.

* `var pdusArray = (Java.Lang.Object[])pdus` This line converts the retrieved PDUs into an array of Java.Lang.Object. These will be converted into SMS message objects.

### Foreground Service

To enable continuous SMS monitoring functionality, it necessitates the incorporation of a foreground service within the application. This service serves the dual purpose of discreetly running in the background and providing a user-facing notification, thus ensuring transparency in app operation even when it's not actively open.

To make this process possible a XAML tag has to be added to the `AndroidManifest` file and the service has to be declared and started int the `MainActivity` file.

`AndroidManifest.xaml`
```
  <uses-permission android:name="android.permission.FOREGROUND_SERVICE" />

```
`MainActivity.cs`

```
var serviceIntent = new Intent(this, typeof(ForgroundServiceSMS));
StartService(serviceIntent);

```

#### ForegroundServiceSMS class    

In order to access the methods required for creating notifications when the application is not in active use, it is necessary to derive the 'ForegroundServiceSMS' class from the 'Service' class.

Explaining the class:

1.The `CreateNotificationChannel` method is creating a notification channel named `Magic Door` with a specified importance level. It ensures that this channel is only created on Android devices with an API level of Oreo or higher. Notification channels allow developers to categorize and control how notifications are presented to users on their Android devices

```
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
```
* `var channel = new NotificationChannel(CHANNEL_ID, channelName, NotificationImportance.Default) { Description = channelDescription }` This creates a new `NotificationChannel` object named channel with the specified `CHANNEL_ID`, `channelName`, and `NotificationImportance`.

* `var notificationManager = GetSystemService(NotificationService) as NotificationManager` This obtains the `NotificationManager `system service using the `GetSystemService` method and assigns it to the `notificationManager` variable. The `NotificationManager` is responsible for managing notifications on the Android device.

* `notificationManager.CreateNotificationChannel(channel)` This creates the notification channel by calling the `CreateNotificationChannel` method of the `notificationManager` and passing the channel object as an argument.


2. The `OnStartCommand` method sets up the `ForegroundServiceSMS` service for SMS processing, creates a notification channel and notification, and runs the service in the foreground to ensure its stability and continuous operation, even when the app is in the background or closed.

```
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
```

* `NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this, CHANNEL_ID)` This creates a notification builder using the `NotificationCompat.Builder` class.It specifies the `CHANNEL_ID` to associate the notification with the previously created notification channel.
*  `.SetContentTitle("Magic Door Service").SetContentText("Service is running in the background").SetSmallIcon(Resource.Drawable.ic_notification_icon)` These chained method calls configure the notification's title, content text, and small icon. The title is "Magic Door Service," the content text is "Service is running in the background," and a custom small icon is set.
* `Notification notification = notificationBuilder.Build()` This builds the notification using the configuration set in the `notificationBuilder` and assigns it to the notification variable.
* `StartForeground(NOTIFICATION_ID, notification)`This method call sets the service as a foreground service, which means it runs with higher priority and is less likely to be terminated by the Android system. It requires a notification to be associated with the service, and that's what the notification variable represents.






### How the app looks like

#### Asking for permissions
![Asking for permissions](https://raw.githubusercontent.com/Cozinia/MagicDoor/main/img/Screenshot_20230910-010539.png?token=GHSAT0AAAAAACHL6VS24OREZ5MHCDOCHGBKZH46NZQ)


#### The user interface of the app
![UI](https://raw.githubusercontent.com/Cozinia/MagicDoor/main/img/Screenshot_2023-09-08-19-23-55-66_a613145c72a70e0166ebad4f316d49e5.jpg?token=GHSAT0AAAAAACHL6VS2X3PM2V6NQA7HM7EOZH46SYA)

#### Notification of the app running in the background
Depending on the Android you're running, the notification should look like this

![Andorid 10](https://raw.githubusercontent.com/Cozinia/MagicDoor/main/img/Screenshot_1694298442.png?token=GHSAT0AAAAAACHL6VS2NZAPCI4VOTNWL74WZH46O5A)

or like this

![Andorid 13](https://raw.githubusercontent.com/Cozinia/MagicDoor/main/img/Screenshot_1694297643.png?token=GHSAT0AAAAAACHL6VS2IAL4LOLKIAZUEO2IZH46QJA)



