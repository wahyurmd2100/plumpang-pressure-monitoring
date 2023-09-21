using System.ComponentModel.DataAnnotations;

namespace TMS.Web.Models
{
    public class Historical
    {
        [Key]
        public int HistoricalID { get; set; }
        public string LocationName { get; set; }
        public double Pressure { get; set; }
        public int TimeStamp { get; set; }
    }

}
