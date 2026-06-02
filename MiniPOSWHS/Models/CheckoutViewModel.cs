namespace MiniPOSWHS.Models;

public class CheckoutViewModel
{
    public int ProductId { get; set; }

    public required string ProductCode { get; set; }

    public required string ProductName { get; set; }

    public decimal Price { get; set; }

    public int Qty { get; set; }
}