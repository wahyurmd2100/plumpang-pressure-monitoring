using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressMon.SchedulerWA.WaModels
{
    public class Contact
    {
        public int contactID { get; set; }
        public string contactName { get; set; }
        public string contactNumber { get; set; }
    }

    public class Data
    {
        public int id { get; set; }
        public string locationName { get; set; }
        public double pressure { get; set; }
        public long timeStamp { get; set; }
    }

    public class PressMonResponse
    {
        public List<Contact> contacts { get; set; }
        public List<Data> data { get; set; }
    }


}
