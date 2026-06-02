using System.ComponentModel.DataAnnotations;

namespace MiniPOSWHS.Models;

public class SaleDetail
{
    public int SaleDetailId { get; set; }

    [Required]
    public int SaleId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Qty { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
}