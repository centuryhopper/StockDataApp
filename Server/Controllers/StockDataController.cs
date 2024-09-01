using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices.Marshalling;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Server.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockDataController : ControllerBase
{
    private readonly ILogger<StockDataController> _logger;
    private readonly IConfiguration configuration;
    private readonly HttpClient httpClient;
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly UserManager<ApplicationUser> userManager;

    public StockDataController(ILogger<StockDataController> logger, IConfiguration configuration, HttpClient httpClient, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        this.configuration = configuration;
        this.httpClient = httpClient;
        this.webHostEnvironment = webHostEnvironment;
        this.userManager = userManager;
    }

    [HttpGet]
    [Route("test")]
    public async Task<IActionResult> Test()
    {
        var users = await userManager.Users.ToListAsync();
        return Ok(users);
    }

    [HttpGet]
    [Route("historic-stock-data")]
    public async Task<IActionResult> GetHistoricalData([FromQuery] string tickerSymbol)
    {
        var apiUrl = webHostEnvironment.IsDevelopment() ? 
        $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={tickerSymbol}&apikey={configuration["Keys:ALPHA_VANTAGE_ApiKey"]}" :  
        $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={tickerSymbol}&outputsize=full&apikey={configuration["Keys:ALPHA_VANTAGE_ApiKey"]}";

        JObject result;

        try
        {
            var response = await httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("response was not successful");
            }
            var responseString = await response.Content.ReadAsStringAsync();
            result = JObject.Parse(responseString);
        }
        catch (System.Exception ex)
        {
            return BadRequest(ex.Message);
        }

        HistoricalStockData historicalStockData = result.ToObject<HistoricalStockData>()!;


        return Ok(historicalStockData);
    }

    [HttpGet]
    [Route("stock-data")]
    public async Task<IActionResult> GetData([FromQuery] string tickerSymbol)
    {
        var apiUrl = webHostEnvironment.IsDevelopment() ? 
        $"https://finnhub.io/api/v1/quote?symbol={tickerSymbol}&token={configuration["Keys:FinnHubApiKey"]}" :  
        $"https://finnhub.io/api/v1/quote?symbol={tickerSymbol}&token={Environment.GetEnvironmentVariable("Keys:FinnHubApiKey")}";

        JObject result;

        try
        {
            var response = await httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("response was not successful");
            }
            var responseString = await response.Content.ReadAsStringAsync();
            result = JObject.Parse(responseString);
        }
        catch (System.Exception ex)
        {
            return BadRequest(ex.Message);
        }

        StockInfo convertedObj = result.ToObject<StockInfo>()!;

        return Ok(convertedObj);
    }
}


/*
"c": 117.59 — The current price of the stock.
"d": -8.02 — The change in the stock's price from the previous closing price.
"dp": -6.3848 — The percentage change in the stock's price from the previous closing price.
"h": 124.43 — The highest price of the stock during the trading day.
"l": 116.72 — The lowest price of the stock during the trading day.
"o": 121.355 — The opening price of the stock at the start of the trading day.
"pc": 125.61 — The previous closing price of the stock.
"t": 1724961600 — The UNIX timestamp for the data, which represents the time when this data was last updated.
*/