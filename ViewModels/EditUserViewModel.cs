using System.ComponentModel.DataAnnotations;

public class EditUserViewModel
{
    public EditUserViewModel()
    {
        Claims = new List<string>();
        Roles = new List<string>();
    }

    public string Id { get; set; }

    [Required]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Country { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public char? Gender { get; set; }

    public List<string> Claims { get; set; }

    public IList<string> Roles { get; set; }
}