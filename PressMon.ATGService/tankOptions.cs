using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressMon.Service
{
    public class tankConfiguration
    {
        public string tankIp { get; set; }
        public int tankPort { get; set; }
        public List<TankDetail> tankDetail { get; set; }
    }
    public class TankDetail
    {
        public int TankId { get; set; }
        public string TankName { get; set; }
        public int startAddr { get; set; }
        public int stopAddr { get; set; }
    }
}
