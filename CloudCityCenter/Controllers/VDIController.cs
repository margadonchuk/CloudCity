using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;

namespace CloudCityCenter.Controllers;

public class VDIController : Controller
{
    private readonly ApplicationDbContext _context;

    public VDIController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // VDI products will be added later
        var products = new List<ProductCardVm>();
        return View(products);
    }
}

