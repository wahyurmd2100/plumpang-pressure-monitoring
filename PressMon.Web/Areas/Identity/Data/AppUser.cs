using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;

namespace PressMon.Web.Areas.Identity.Data
{
    public class AppUser: IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "varchar(100)")]
        [Required(ErrorMessage = "Nama lengkap wajib diisi")]
        [Display(Name = "Nama Lengkap")]
        public string FullName { get; set; }
    }
}
