using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MagicDoor
{
    public class Contacts
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}
