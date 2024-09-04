

namespace Shared.Models;

public class StockDataDTO
{
    public int Userid { get; set; }
    public string TickerSymbol { get; set; } = null!;
    public decimal? OpenPrice { get; set; }
    public decimal? ClosePrice { get; set; }
    public decimal? HighPrice { get; set; }
    public decimal? LowPrice { get; set; }
    public DateOnly DateCreated { get; set; }
    public decimal? CurrentPrice { get; set; }
    public decimal? Change { get; set; }
    public decimal? PercentChange { get; set; }
    public decimal? PreviousClose { get; set; }
}