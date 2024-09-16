using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IntexLegoSecure.Models;


public partial class Order
{
    [Key]
    public int TransactionId { get; set; }

    public int? CustomerId { get; set; }

    public DateOnly? Date { get; set; }

    public string? DayOfWeek { get; set; }

    public int? Time { get; set; }

    public string? EntryMode { get; set; }

    public int? Amount { get; set; }

    public string? TransactionType { get; set; }

    public string? TransactionCountry { get; set; }

    public string? ShippingCountry { get; set; }

    public string? ShippingAddress { get; set; }

    public string? ShippingCity { get; set; }

    public string? ShippingState { get; set; }

    public string? Bank { get; set; }

    public string? CardType { get; set; }

    public int? Fraud { get; set; }
}
