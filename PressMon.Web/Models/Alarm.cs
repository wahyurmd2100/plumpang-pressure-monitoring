using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS.Web.Models
{
    public class Alarm
    {
        [Key]
        public int AlarmID { get; set; }
        public string AlarmStatus { get; set; }
        public string LocationName { get; set; }
        public double Pressure { get; set; }
        public int TimeStamp { get; set; }
        public virtual WaAlarmMessageStatus WaAlarmMessageStatus { get; set; }
    }
}
