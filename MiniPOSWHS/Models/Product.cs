namespace MiniPOSWHS.Models;

public class Product
{
    public int ProductId { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int StockQty { get; set; }

    public DateTime CreatedDate { get; set; }
}