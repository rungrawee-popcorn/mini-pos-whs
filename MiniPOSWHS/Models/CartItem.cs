namespace MiniPOSWHS.Models;

public class CartItem
{
    public int ProductId { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Qty { get; set; }

    public decimal Total => Price * Qty;
}