using System.ComponentModel.DataAnnotations;

namespace MvcJwtAuthApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
