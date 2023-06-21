using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressMon.Service
{
    public class SensorConfig
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public int TimeLoop { get; set; }
        public List<Sensor> Sensors { get; set; }
    }
    public class Sensor
    {
        public int Address { get; set; }
        public string Type { get; set; }
        public double Value { get; set; }
        public string Location { get; set; }
    }
}
