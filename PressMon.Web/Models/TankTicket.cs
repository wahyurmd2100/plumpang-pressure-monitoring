using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel;

namespace PressMon.Web.Models
{
    public class TankTicket
    {

        [Key]
        public int Id { get; set; }

        [Display(Name = "Ticket Number")]
        public string Ticket_Number { get; set; }
        [Display(Name = "Date and Time")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Timestamp { get; set; }
        [Display(Name = "Do Number")]
        public string Do_Number { get; set; }
        [Display(Name = "Shipment")]
        public string Shipment_Id { get; set; }
        [Display(Name = "Tank")]
        public int Tank_Id { get; set; }
        [Display(Name = "Product")]
        public int? Product_Id { get; set; }
        [Display(Name = "Operation Type")]
        public string Operation_Type { get; set; }

        [Display(Name = "Status")]
        public int Operation_Status { get; set; }
        [Display(Name = "Measurement Method")]
        public string Measurement_Method { get; set; }

        [Display(Name = "Liquid Level")]
        public double Liquid_Level { get; set; }
        [Display(Name = "Water Level")]
        public double Water_Level { get; set; }
        [Display(Name = "Liquid Temperature")]
        public double Liquid_Temperature { get; set; }
        [Display(Name = "Liquid Density")]
        public double Liquid_Density { get; set; }
        [Display(Name = "Volume Observed")]
        public double Volume_Observed { get ; set; }
        public double Volume_Net_Standard { get; set; }
        public string? Tank_Ticket_Pair { get; set; }
        
        public int Is_Upload_Success { get; set; }
        [Display(Name ="SAP_Response")]
        [StringLength(500)]
        public string? SAP_Response { get; set; }
        public double Volume_Barrel60F { get; set; }
        public double Volume_LongTon { get; set; }
        
        public double Volume_Product { get; set; }
    
        public double Volume_15C { get; set; }
        [Display(Name = "Create By")]
        public string? Created_By { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Create")]
        public DateTime? Created_Timestamp { get; set; }
        [Display(Name = "Update By")]
        public string? Updated_By { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Create")]
        public DateTime? Updated_Timestamp { get; set; }
    }    
}