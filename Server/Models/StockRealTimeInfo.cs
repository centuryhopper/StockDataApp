

using Newtonsoft.Json;

namespace Shared.Models;


public class StockRealTimeInfo
{
    [JsonProperty("c")]
    public decimal CurrentPrice { get; set; }

    [JsonProperty("d")]
    public decimal Change { get; set; }

    [JsonProperty("dp")]
    public decimal PercentChange { get; set; }

    [JsonProperty("h")]
    public decimal High { get; set; }

    [JsonProperty("l")]
    public decimal Low { get; set; }

    [JsonProperty("o")]
    public decimal Open { get; set; }

    [JsonProperty("pc")]
    public decimal PreviousClose { get; set; }

    [JsonProperty("t")]
    public long Timestamp { get; set; }
}