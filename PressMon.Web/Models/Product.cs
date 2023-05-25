using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace PressMon.Web.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Product Code")]
        public string ProductCode { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Hex Color")]
        public string HexColor { get; set; }

        [Display(Name = "Default Density  (kg/m³)")]
        public double DefaultDensity { get; set; }

        [Display(Name = "Default Temp (°C)")]
        public double DefaultTemp { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Create ")]
        public DateTime? CreateTime { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Update")]
        public DateTime? UpdateTime { get; set; }

        [Display(Name = "Create By")]
        public string CreateBy { get; set; }

        [Display(Name = "Update By")]
        public string UpdateBy { get; set; }
    }
}
