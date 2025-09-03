namespace CloudCityCenter.Models.ViewModels;

public class CartItemViewModel
{
    public OrderItem Item { get; set; } = new();
    public Product? Product { get; set; }
    public ProductVariant? ProductVariant { get; set; }
}

public class CartViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new();
    public decimal Total { get; set; }
}
