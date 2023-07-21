using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PressMon.Web.Areas.Identity.Data
{
    public class AppRole: IdentityRole
    {
        [Column("Update_Timestamp")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date Update")]
        public DateTime? CreateTimeStamp { get; set; }
    }
}
