using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;

namespace TMS.Web.Models.sendWA
{
    public class Response
    {
        public List<contact> contacts { get; set; }
        public List<message> messages { get; set; }
        public string messaging_product { get; set; }
    }
    public class contact 
    {
        public string input { get; set; }
        public string wa_id { get; set; }
    }
    public class message
    {
        public string id { get; set; }
    }
}

