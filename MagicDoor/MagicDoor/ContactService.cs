using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace MagicDoor
{
    public static class ContactService
    {
        static SQLiteAsyncConnection db;
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


        public static async Task<IEnumerable<Contacts>> getContacts()
        {
            await Init();

            var _table_contacts = await db.Table<Contacts>().ToListAsync();
            return _table_contacts;
        }

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

    }
}
