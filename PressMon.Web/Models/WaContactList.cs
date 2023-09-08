using System.ComponentModel.DataAnnotations;

namespace TMS.Web.Models
{
    public class WaContactList
    {
        [Key]
        public int ContactID { get; set; }
        public string ContactName { get; set; }
        public string ContactNumber { get; set; }
    }
}
