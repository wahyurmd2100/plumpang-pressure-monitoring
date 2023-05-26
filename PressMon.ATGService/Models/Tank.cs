﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressMon.Service.Models
{
    public class Tank
    {
        [Key]
        public int TankId { get; set; }
        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public double TankVolume { get; set; }
        public double TankHeight { get; set; }
        public bool IsUsed { get; set; }
        public bool IsAutoPI { get; set; }
        public double TankDiameter { get; set; }
        public string TankForm { get; set; }
        public int HeightSafeCapacity { get; set; }
        public int HeightVolMax { get; set; }
        public int HeightPointDesk { get; set; }
        public int HeightTankBase { get; set; }
        public int HeightDeadStock { get; set; }
        public double StretchCoefficient { get; set; }
        public double DensityCalibrate { get; set; }
        public double RaisePerMM { get; set; }
        public double DeadstockVolume { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Create")]
        public DateTime? CreateTime { get; set; }
        [Display(Name = "Create By")]
        public string CreateBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Update")]
        public DateTime? UpdateTime { get; set; }
        [Display(Name = "Update By")]
        public string UpdateBy { get; set; }

        public virtual TankLiveData tankLiveData { get; set; }

    }
}