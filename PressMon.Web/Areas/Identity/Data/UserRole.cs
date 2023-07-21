using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PressMon.Web.Areas.Identity.Data
{
    public class UserRole: IdentityUserRole<string>
    {
        public UserRole() : base() { }
    }
}
