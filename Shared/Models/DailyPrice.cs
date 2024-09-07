using Newtonsoft.Json;

namespace Shared.Models;

public class DailyPrice
{
    [JsonProperty("1. open")]
    public decimal Open { get; set; }

    [JsonProperty("2. high")]
    public decimal High { get; set; }

    [JsonProperty("3. low")]
    public decimal Low { get; set; }

    [JsonProperty("4. close")]
    public decimal Close { get; set; }

    [JsonProperty("5. adjusted close")]
    public decimal AdjustedClose { get; set; }

    [JsonProperty("6. volume")]
    public long Volume { get; set; }

    [JsonProperty("7. dividend amount")]
    public long DividendAmount { get; set; }

    public override string ToString()
    {
        return $"{nameof(Open)}:{Open}, {nameof(High)}:{High}, {nameof(Low)}:{Low}, {nameof(Close)}:{Close}, {nameof(Volume)}:{Volume},";
    }
}
