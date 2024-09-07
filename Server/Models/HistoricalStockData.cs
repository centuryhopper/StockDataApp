using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shared.Models;

public class HistoricalStockData
{
    [JsonProperty("Meta Data")]
    public MetaData MetaData { get; set; }

    // The key for the dictionary is the date string from the JSON
    [JsonProperty("Monthly Adjusted Time Series")]
    public Dictionary<string, DailyPrice> DailyPrices { get; set; }
}
