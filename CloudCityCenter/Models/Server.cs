using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.RegularExpressions;

namespace CloudCityCenter.Models;

public class Server
{
    [Key]
    public int Id { get; set; }

    private string _name = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            if (string.IsNullOrWhiteSpace(_slug))
            {
                _slug = GenerateSlug(_name);
            }
        }
    }

    private string _slug = string.Empty;

    [Required]
    [StringLength(100)]
    public string Slug
    {
        get => _slug;
        set => _slug = string.IsNullOrWhiteSpace(value) ? GenerateSlug(Name) : GenerateSlug(value);
    }

    [StringLength(200)]
    public string? Description { get; set; }

    [Required]
    [StringLength(100)]
    public string? Location { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PricePerMonth { get; set; }

    [StringLength(100)]
    public string CPU { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int RamGb { get; set; }

    [Range(0, int.MaxValue)]
    public int StorageGb { get; set; }

    private string? _imageUrl;

    [StringLength(300)]
    public string? ImageUrl
    {
        get => _imageUrl;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _imageUrl = null;
            }
            else
            {
                var fileName = Path.GetFileName(value);
                _imageUrl = $"/images/servers/{fileName}";
            }
        }
    }

    public bool IsActive { get; set; } = true;

    [Required]
    [StringLength(50)]
    public string DDoSTier { get; set; } = "Basic";

    [Range(0, int.MaxValue)]
    public int Stock { get; set; } = 9999;

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    private static string GenerateSlug(string value)
    {
        var slug = value.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", " ").Trim();
        slug = slug.Replace(" ", "-");
        return slug;
    }
}
