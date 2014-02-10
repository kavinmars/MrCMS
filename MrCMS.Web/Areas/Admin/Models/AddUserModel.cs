using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.People;
using MrCMS.Helpers;

namespace MrCMS.Web.Areas.Admin.Models
{
    [DoNotMap]
    public class AddUserModel : User
    {
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Minimum length for password is {2} characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password does not match.")]
        public string ConfirmPassword { get; set; }
    }
}