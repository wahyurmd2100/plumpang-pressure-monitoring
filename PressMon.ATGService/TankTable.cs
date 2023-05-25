using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressMon.Service
{
    public class TankTable
    {
        public string TankName { get; set; }
        public List<TankTableDetail> TankTableDetails { get; set; }
    }
    public class TankTableDetail
    {
        public double Level { get; set; }
        public double Volume { get; set; }
    }
}
