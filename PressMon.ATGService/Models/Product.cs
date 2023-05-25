using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressMon.Service.Models
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
        [Display(Name = "Default Density  (kg/m&sup3;)")]
        public double DefaultDensity { get; set; }

        [Display(Name = "Default Temp (&deg;C)")]
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
