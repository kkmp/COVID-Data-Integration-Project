using System.ComponentModel.DataAnnotations;

namespace Projekt.Models
{
    public class UserViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
