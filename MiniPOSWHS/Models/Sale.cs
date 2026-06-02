namespace MiniPOSWHS.Models;

public class Sale
{
    public int SaleId { get; set; }

    public string SaleNo { get; set; } = string.Empty;

    public DateTime SaleDate { get; set; }

    public decimal TotalAmount { get; set; }

    public int UserId { get; set; }
}