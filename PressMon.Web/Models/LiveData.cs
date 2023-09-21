using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS.Web.Models
{
    public class LiveData
    {
        [Key]
        public int ID { get; set; }
        public string LocationName { get; set; }
        public double Pressure { get; set; }
        public int TimeStamp { get; set; }
    }
}
