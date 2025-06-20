using System.Collections.Generic;

namespace CloudCityCenter.Models.ViewModels;

public class ServiceIndexViewModel
{
    public IEnumerable<Server> Servers { get; set; } = new List<Server>();
}
