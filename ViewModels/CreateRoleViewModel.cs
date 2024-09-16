using System.ComponentModel.DataAnnotations;

namespace IntexLegoSecure.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
