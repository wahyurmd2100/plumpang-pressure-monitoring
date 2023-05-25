using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace PressMon.Models
{
    public class TankLiveData
    {
        //Foreign Key
        [Key, ForeignKey("Tank")]
        public int TankId { get; set; }
        [DisplayName("Liquid Level (mm)")]
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public double LiquidLevel { get; set; }
        [DisplayName("Water Level")]
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public double WaterLevel { get; set; }
        [DisplayName("Liquid Temperature")]
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public double LiquidTemperature { get; set; }
        [DisplayName("Liquid Density")]
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public double LiquidDensity { get; set; }
        [DisplayName("Volume Observed")]
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public double VolumeObserved { get; set; }
        [DisplayName("Volume Net Standard")]
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public double VolumeNetStandard { get; set; }
        [DisplayName("Timestamp")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? TimeStamp { get; set; }

        public virtual Tank tank { get; set; }
    }
}
