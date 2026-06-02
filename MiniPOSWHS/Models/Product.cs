using System.ComponentModel.DataAnnotations;

namespace MiniPOSWHS.Models;

public class Product
{
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Product Code is required")]
    public required string ProductCode { get; set; }

    [Required(ErrorMessage = "Product Name is required")]
    public required string ProductName { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Stock Qty is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
    public int StockQty { get; set; }

    public DateTime CreatedDate { get; set; }
}