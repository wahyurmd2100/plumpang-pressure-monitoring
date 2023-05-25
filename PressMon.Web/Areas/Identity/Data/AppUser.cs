using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PressMon.Web.Models;

namespace PressMon.Web.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the AppUser class
    public class AppUser : IdentityUser // ini inheritence namanya man. kalau rule buat lagi modelnya pakai inheritence
    {
        [PersonalData]
        [Column(TypeName = "varchar(100)")]
        [Required(ErrorMessage = "Nama lengkap wajib diisi")]
        [Display(Name = "Nama Lengkap")]
        public string FullName { get; set; }

        [PersonalData]
        [Display(Name = "Upload Foto")]
        public byte[] UserPhoto { get; set; }
    }
}
