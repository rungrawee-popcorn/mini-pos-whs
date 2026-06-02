namespace MiniPOSWHS.ViewModels;

public class SaleDetailViewModel
{
    public int SaleId { get; set; }

    public string SaleNo { get; set; } = string.Empty;

    public DateTime SaleDate { get; set; }

    public decimal TotalAmount { get; set; }

    public List<SaleDetailItemViewModel> Items { get; set; } = new();
}

public class SaleDetailItemViewModel
{
    public string ProductCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public int Qty { get; set; }

    public decimal Price { get; set; }

    public decimal Amount { get; set; }
}