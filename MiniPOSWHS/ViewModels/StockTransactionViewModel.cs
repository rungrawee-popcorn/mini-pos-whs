namespace MiniPOSWHS.ViewModels;

public class StockTransactionViewModel
{
    public int TransactionId { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string TransactionType { get; set; } = string.Empty;

    public int Qty { get; set; }
    
    public string Source { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
}