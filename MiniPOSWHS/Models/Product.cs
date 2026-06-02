using System.ComponentModel.DataAnnotations;

namespace MiniPOSWHS.Models;

public class Product
{
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Product Code is required")]
    [StringLength(20, ErrorMessage = "Product Code must not exceed 20 characters")]
    public string ProductCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Product Name is required")]
    [StringLength(100, ErrorMessage = "Product Name must not exceed 100 characters")]
    public string ProductName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Stock is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
    public int StockQty { get; set; }

    public DateTime CreatedDate { get; set; }
}