namespace MiniPOSWHS.Models;

public class DashboardViewModel
{
    public int TotalProducts { get; set; }

    public decimal TotalSalesToday { get; set; }

    public int TotalTransactions { get; set; }

    public int LowStockProducts { get; set; }

    public List<DailySalesItem> DailySales { get; set; } = new();

    public List<TopSellingProductItem> TopSellingProducts { get; set; } = new();

    public List<LowStockProductItem> LowStockItems { get; set; } = new();
}

public class DailySalesItem
{
    public DateTime SaleDate { get; set; }

    public decimal TotalAmount { get; set; }
}

public class TopSellingProductItem
{
    public string ProductCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public int TotalQty { get; set; }
}

public class LowStockProductItem
{
    public string ProductCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public int StockQty { get; set; }
}