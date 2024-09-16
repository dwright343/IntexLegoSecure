using Microsoft.AspNetCore.Identity;

namespace IntexLegoSecure.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Country { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public char? Gender { get; set; }
    }
}
