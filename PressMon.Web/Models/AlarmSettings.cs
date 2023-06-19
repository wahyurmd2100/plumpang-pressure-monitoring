using System.ComponentModel.DataAnnotations;


namespace TMS.Web.Models
{
    public class AlarmSettings
    {
        [Key]
        public int AlarmSettingID { get; set; }
        public double Value { get; set; }
        public string Info { get; set; }
        public int UpdateTimestamp { get; set; }
    }
}
