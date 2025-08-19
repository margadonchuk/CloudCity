using System.Collections.Generic;

namespace CloudCityCenter.Models.ViewModels;

public class ServiceIndexViewModel
{
    public IEnumerable<Product> Products { get; set; } = new List<Product>();
}
