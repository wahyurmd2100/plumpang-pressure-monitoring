using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TMS.ATGService.Models
{
    public class TankLiveData
    {
        //Foreign Key
        [Key, ForeignKey("Tank")]
        public int TankId { get; set; }

        [DisplayName("Level (mm)")]
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public double Level { get; set; }

        [DisplayName("Temperature (°C)")]
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public double Temperature { get; set; }

        [DisplayName("Gross Volume (L)")]
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public double GrossVolume { get; set; }

        [DisplayName("Net Volume (L)")]
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public double NetVolume { get; set; }

        [DisplayName("Density (g/cm³)")]
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public double Density { get; set; }

        [DisplayName("V.C.F")]
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public double VCF { get; set; }

        [DisplayName("Timestamp")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? TimeStamp { get; set; }

        [DisplayName("Liquid Weight (M. Ton)")]
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public double LiquidWeight { get; set; }

        //[DisplayName("Error")]
        //public double Error { get; set; }

        public virtual Tank tank { get; set; }
    }
}
