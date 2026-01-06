using System;
using System.ComponentModel.DataAnnotations;

namespace CloudCityCenter.Models;

public class ContactMessage
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Subject { get; set; }

    [StringLength(100)]
    public string? ServiceType { get; set; }

    [Required]
    public string Message { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string SourcePage { get; set; } = string.Empty; // "About" или "Contact"

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsRead { get; set; } = false;

    public DateTime? ReadAt { get; set; }
}

