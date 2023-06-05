namespace TMS.Web.Models
{
    public class Alarm
    {
        public int AlarmID { get; set; }
        public string AlarmStatus { get; set; }

        public string LoacationName { get; set; }
        public double Pressure { get; set; }
        public int TimeStamp { get; set; }
    }
}
