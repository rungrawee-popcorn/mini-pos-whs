namespace MiniPOSWHS.Models;

public class SaleDetail
{
    public int SaleDetailId { get; set; }

    public int SaleId { get; set; }

    public int ProductId { get; set; }

    public int Qty { get; set; }

    public decimal Price { get; set; }

    public decimal Amount { get; set; }
}