using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TMS.Web.Models
{
    public class WaContactList
    {
        [Key]
        public int ContactID { get; set; }
        public string ContactName { get; set; }
        public string ContactNumber { get; set; }
        public bool CheckStatus { get; set; }
        public bool IsActived { get; set; }
    }
}
