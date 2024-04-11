using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IntexLegoSecure.Models;

public partial class Customer
{
    [Key]
    public int CustomerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string CountryOfResidence { get; set; } = null!;

    public string? Gender { get; set; }

    public int Age { get; set; }
}
