using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices.Marshalling;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Server.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using static Shared.Models.ServiceResponses;
using System.Runtime.Intrinsics.X86;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
//? I didn't feel like using repository pattern for stock data CRUD so I inject the db context directly in this controller
public class StockDataController : ControllerBase
{
    private readonly ILogger<StockDataController> _logger;
    private readonly IConfiguration configuration;
    private readonly HttpClient httpClient;
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly StockDataDbContext stockDataDbContext;

    public StockDataController(ILogger<StockDataController> logger, IConfiguration configuration, HttpClient httpClient, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager, StockDataDbContext stockDataDbContext)
    {
        _logger = logger;
        this.configuration = configuration;
        this.httpClient = httpClient;
        this.webHostEnvironment = webHostEnvironment;
        this.userManager = userManager;
        this.stockDataDbContext = stockDataDbContext;
    }

    [HttpGet("admin_test")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminTest()
    {
        // var users = await userManager.Users.ToListAsync();
        var userId = Convert.ToInt32(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        return Ok(userId);
    }

    [HttpGet("user_test")]
    [Authorize(Roles = "Normal_User")]
    public IActionResult UserTest()
    {
        var userId = Convert.ToInt32(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        return Ok(userId);
    }

    [HttpPost("save-stock-data")]
    [Authorize(Roles = "Admin,Normal_User")]
    public async Task<IActionResult> SaveStockData([FromBody] StockDataDTO stockDataDTO)
    {
        try
        {
            if (await stockDataDbContext.Stockdata.FirstOrDefaultAsync(s => s.TickerSymbol == stockDataDTO.TickerSymbol) is null)
            {
                await stockDataDbContext.Stockdata.AddAsync(new Stockdatum
                {
                    Userid = stockDataDTO.Userid
                    ,
                    TickerSymbol = stockDataDTO.TickerSymbol
                    ,
                    OpenPrice = stockDataDTO.OpenPrice
                    ,
                    ClosePrice = stockDataDTO.ClosePrice
                    ,
                    HighPrice = stockDataDTO.HighPrice
                    ,
                    LowPrice = stockDataDTO.LowPrice
                    ,
                    DateCreated = stockDataDTO.DateCreated
                    ,
                    CurrentPrice = stockDataDTO.CurrentPrice
                    ,
                    Delta = stockDataDTO.Change
                    ,
                    PercentDelta = stockDataDTO.PercentChange
                    ,
                    PreviousClose = stockDataDTO.PreviousClose
                });
                await stockDataDbContext.SaveChangesAsync();
            }
        }
        catch (System.Exception ex)
        {
            return BadRequest(new GeneralResponse(false, ex.Message));
        }

        return Ok(new GeneralResponse(true, "saved!"));
    }

    [HttpGet("stored-stock-data")]
    [Authorize(Roles = "Admin,Normal_User")]
    public async Task<IActionResult> GetStoredStockData()
    {
        var userId = Convert.ToInt32(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        var stockDataLst = await stockDataDbContext.Stockdata.Where(s => s.Userid == userId).Select(s => new StockDataDTO
        {
            StockDataId = s.Stockdataid,
            Userid = s.Userid,
            TickerSymbol = s.TickerSymbol,
            OpenPrice = s.OpenPrice,
            ClosePrice = s.ClosePrice,
            HighPrice = s.HighPrice,
            LowPrice = s.LowPrice,
            DateCreated = s.DateCreated,
            CurrentPrice = s.CurrentPrice,
            Change = s.Delta,
            PercentChange = s.PercentDelta,
            PreviousClose = s.PreviousClose,
        }).ToListAsync();

        return Ok(stockDataLst);
    }

    [HttpGet("get-stock-by-ticker/{tickerSymbol}")]
    public async Task<IActionResult> GetStockDataByTicker(string tickerSymbol)
    {
        var stockData = await stockDataDbContext.Stockdata.FirstOrDefaultAsync(s=>s.TickerSymbol == tickerSymbol);
        if (stockData is null)
        {
            return BadRequest("ticker not found in the database");
        }

        // System.Console.WriteLine(stockData.TickerSymbol);
        // System.Console.WriteLine(stockData.Stockdataid);

        return Ok(new StockDataDTO {
            StockDataId = stockData.Stockdataid,
            Userid = stockData.Userid,
            TickerSymbol = stockData.TickerSymbol,
            OpenPrice = stockData.OpenPrice,
            ClosePrice = stockData.ClosePrice,
            HighPrice = stockData.HighPrice,
            LowPrice = stockData.LowPrice,
            DateCreated = stockData.DateCreated,
            CurrentPrice = stockData.CurrentPrice,
            Change = stockData.Delta,
            PercentChange = stockData.PercentDelta,
            PreviousClose = stockData.PreviousClose,
        });
    }

    [HttpPut("update-stock-data")]
    [Authorize(Roles = "Admin,Normal_User")]
    public async Task<IActionResult> UpdateStockData([FromBody] StockDataDTO stockDataDTO)
    {
        var stockData = await stockDataDbContext.Stockdata.FindAsync(stockDataDTO.StockDataId);
        if (stockData is null)
        {
            return BadRequest(new GeneralResponse(false, "Couldn't find the stock you're trying to update"));
        }
        stockData.CurrentPrice = stockDataDTO.CurrentPrice;
        stockData.Delta = stockDataDTO.Change;
        stockData.PercentDelta = stockDataDTO.PercentChange;
        stockData.OpenPrice = stockDataDTO.OpenPrice;
        stockData.PreviousClose = stockDataDTO.PreviousClose;
        stockData.HighPrice = stockDataDTO.HighPrice;
        stockData.LowPrice = stockDataDTO.LowPrice;
        await stockDataDbContext.SaveChangesAsync();

        return Ok(new GeneralResponse(true, "Stock Updated!"));
    }

    [HttpGet("historical-stock-data")]
    public async Task<IActionResult> GetHistoricalData([FromQuery] string tickerSymbol)
    {
        var apiUrl = string.Format("https://www.alphavantage.co/query?function=TIME_SERIES_MONTHLY_ADJUSTED&symbol={0}&apikey={1}", tickerSymbol, webHostEnvironment.IsDevelopment() ? configuration["Keys:ALPHA_VANTAGE_ApiKey"] : Environment.GetEnvironmentVariable("ALPHA_VANTAGE_ApiKey"));

        // $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={tickerSymbol}&outputsize=full&apikey={Environment.GetEnvironmentVariable("ALPHA_VANTAGE_ApiKey")}";

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

        try
        {
            HistoricalStockData historicalStockData = result.ToObject<HistoricalStockData>()!;
            // System.Console.WriteLine(historicalStockData.MetaData);
            // historicalStockData.DailyPrices.ToList().ForEach((KeyValuePair<string, DailyPrice> kvp)=>System.Console.WriteLine(kvp.Key));
            return Ok(historicalStockData);
        }
        catch (Exception ex)
        {
            HistoricalStockDataRateLimit historicalStockDataRateLimit = result.ToObject<HistoricalStockDataRateLimit>()!;
            System.Console.WriteLine(historicalStockDataRateLimit.Information);
            return StatusCode(500, historicalStockDataRateLimit);
        }
    }

    [HttpDelete("delete-stock-data/{stockDataId:int}")]
    public async Task<ActionResult> DeleteAsync(int stockDataId)
    {
        var stockData = await stockDataDbContext.Stockdata.FindAsync(stockDataId);
        if (stockData == null)
        {
            return BadRequest(new GeneralResponse(false, "Stock not found"));
        }

        stockDataDbContext.Stockdata.Remove(stockData);
        await stockDataDbContext.SaveChangesAsync();

        return Ok(new GeneralResponse(true, "Stock Deleted!"));
    }

    [HttpGet("stock-realtime-data")]
    public async Task<IActionResult> GetData([FromQuery] string tickerSymbol)
    {
        var apiUrl = webHostEnvironment.IsDevelopment() ?
        $"https://finnhub.io/api/v1/quote?symbol={tickerSymbol}&token={configuration["Keys:FinnHubApiKey"]}" :
        $"https://finnhub.io/api/v1/quote?symbol={tickerSymbol}&token={Environment.GetEnvironmentVariable("FinnHubApiKey")}";

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

        StockRealTimeInfo convertedObj = result.ToObject<StockRealTimeInfo>()!;

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