using Microsoft.AspNetCore.Mvc;
using PaginationMvcApp.Data;

namespace PaginationMvcApp.Controllers;

public class ProductController : Controller
{
    private readonly AppDbContext _context;
    private const int PageSize = 5;

    public ProductController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(int page = 1)
    {
        if (page < 1)
            page = 1;

        int totalProducts = _context.Products.Count();
        int totalPages = (int)Math.Ceiling((double)totalProducts / PageSize);

        if (page > totalPages && totalPages > 0)
            page = totalPages;

        var products = _context.Products
            .OrderBy(p => p.Id)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        ViewData["CurrentPage"] = page;
        ViewData["TotalPages"] = totalPages;
        ViewData["TotalProducts"] = totalProducts;

        return View(products);
    }
}
