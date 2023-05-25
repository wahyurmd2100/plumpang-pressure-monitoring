using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CSL.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CSL.Web.Models
{
    public class UserRole
    {
        
        public AppUser ApplicationUser { get; set; }
        public List<SelectListItem> ApplicationRoles { get; set; }

    }
}
