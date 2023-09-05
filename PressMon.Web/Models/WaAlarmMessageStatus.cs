using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS.Web.Models
{
    public class WaAlarmMessageStatus
    {
        [Key]
        public int AlarmId { get; set; }
        public int status { get; set; }
        public string ApiResponse { get; set; }

        [ForeignKey("AlarmId")]
        public virtual Alarm Alarm { get; set; } // Navigation property
    }
}
