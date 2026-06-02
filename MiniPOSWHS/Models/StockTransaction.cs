using System.ComponentModel.DataAnnotations;

namespace MiniPOSWHS.Models;

public class StockTransaction
{
    [Key]
    public int TransactionId { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public string TransactionType { get; set; } = string.Empty;

    public int Qty { get; set; }

    public DateTime CreatedDate { get; set; }
}