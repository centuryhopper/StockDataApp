
using Newtonsoft.Json;

namespace Shared.Models;

public class MetaData
{
    [JsonProperty("1. Information")]
    public string Information { get; set; }

    [JsonProperty("2. Symbol")]
    public string Symbol { get; set; }

    [JsonProperty("3. Last Refreshed")]
    public DateTime LastRefreshed { get; set; }

    [JsonProperty("4. Time Zone")]
    public string TimeZone { get; set; }
}