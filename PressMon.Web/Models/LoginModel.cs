using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations;

namespace TMS.Web.Models
{
    public class LoginModel
    {

    }
    public class InputModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Ingatkan saya?")]
        public bool RememberMe { get; set; }
    }
}
