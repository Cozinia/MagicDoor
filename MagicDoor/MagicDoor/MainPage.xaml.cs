using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Diagnostics;


namespace MagicDoor
{
    public partial class MainPage : ContentPage
    {
        public class Contact
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
        }
        public MainPage()
        {
            InitializeComponent();

            // Load the previously saved list of contacts from Preferences
            GetContactsAsync();

            nameEntry.Text = string.Empty;
            phoneEntry.Text = string.Empty;

           
        }

        private async void GetContactsAsync()
        {
            try
            {
                // Retrieve and set the contacts asynchronously
                var contacts = await ContactService.getContacts();
                contactListView.ItemsSource = contacts;
                


            }
            catch (Exception ex)
            {
                // Handle any exceptions or errors that occur during contact loading
                Debug.WriteLine("Error loading contacts: " + ex.Message);
            }
        }

        private async void Btn_save_Clicked(object sender, EventArgs e)
        {
            string name = nameEntry.Text;
            string phone = phoneEntry.Text;

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(phone))
            {
                await ContactService.addContact(name, phone);
                // Clear the entry fields after adding the contact
                nameEntry.Text = string.Empty;
                phoneEntry.Text = string.Empty;
                

               
            }
            GetContactsAsync();
        }

        private async void Btn_delete_Clicked(object sender, EventArgs e)
        {
            string id = deleteEntry.Text;

            if (!string.IsNullOrWhiteSpace(id))
            {
                await ContactService.removeContact(System.Convert.ToInt32(id));
                // Clear the entry fields after adding the contact
                deleteEntry.Text = string.Empty;


            }
            GetContactsAsync();
        }

        private async void ContactListView_SentOpen(object sender, ItemTappedEventArgs e)
        {
            var websiteURL = "https://www.google.com/";
            
            if (e.Item is Contacts selectedContact)
            {
                
                Uri uri= new Uri(websiteURL);
                await Launcher.OpenAsync(uri);
            }

            // Reset the selected item (if needed)
            ((ListView)sender).SelectedItem = null;
        }

    }

}
