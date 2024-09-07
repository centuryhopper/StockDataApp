using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Client.Interfaces;
using Client.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Models;
using static Shared.Models.ServiceResponses;

namespace Client.Services;

public class StockDataService : IStockDataService
{
    private readonly HttpClient httpClient;
    private readonly ILocalStorageService localStorageService;

    public StockDataService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
    {
        httpClient = httpClientFactory.CreateClient("API");
        this.localStorageService = localStorageService;
    }

    public async Task<GeneralResponse> UpdateStockData(string tickerSymbol, StockRealTimeInfo stockRealTimeInfo)
    {
        var getCurrentStockDataResponse = await httpClient.GetAsync($"api/StockData/get-stock-by-ticker/{tickerSymbol}");
        if (!getCurrentStockDataResponse.IsSuccessStatusCode)
        {
            var feedback = await getCurrentStockDataResponse.Content.ReadAsStringAsync();
            return new GeneralResponse(false, feedback);
        }


        var stockData = await getCurrentStockDataResponse.Content.ReadFromJsonAsync<StockDataDTO>();
        stockData!.CurrentPrice = stockRealTimeInfo.CurrentPrice;
        // System.Console.WriteLine(stockData.TickerSymbol);
        // System.Console.WriteLine(stockData.StockDataId);
        stockData.Change = stockRealTimeInfo.Change;
        stockData.PercentChange = stockRealTimeInfo.PercentChange;
        stockData.HighPrice = stockRealTimeInfo.High;
        stockData.LowPrice = stockRealTimeInfo.Low;
        stockData.OpenPrice = stockRealTimeInfo.Open;
        stockData.PreviousClose = stockRealTimeInfo.PreviousClose;

        var response = await httpClient.PutAsJsonAsync("api/StockData/update-stock-data", stockData);

        var data = await response.Content.ReadFromJsonAsync<GeneralResponse>();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(data.Message);
        }

        return data!;   

    }
    
    public async Task<IEnumerable<StockDataDTO>> GetStockData()
    {
        var response = await httpClient.GetAsync(
            "api/StockData/stored-stock-data");
        // System.Console.Write(response.StatusCode.ToString());

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var data = await response.Content.ReadFromJsonAsync<IEnumerable<StockDataDTO>>();

        return data!;
    }

    public async Task<HistoricalStockData?> GetStockHistoricalData(string tickerSymbol)
    {
        var response = await httpClient.GetAsync($"api/StockData/historical-stock-data?tickerSymbol={tickerSymbol}");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("couldn't get data for this ticker symbol");
        }

        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            throw new Exception("rate limit exceeded for today");
        }

        var historicalStockData = await response.Content.ReadFromJsonAsync<HistoricalStockData>();

        return historicalStockData;
    }

    public async Task<StockRealTimeInfo?> GetStockRealTimeData(string tickerSymbol)
    {
        var response = await httpClient.GetAsync($"api/StockData/stock-realtime-data?tickerSymbol={tickerSymbol}");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("couldn't get data for this ticker symbol");
        }

        var realTimeInfo = await response.Content.ReadFromJsonAsync<StockRealTimeInfo>();
        return realTimeInfo;
    }

    public async Task<GeneralResponse> SaveStockData(StockDataDTO stockDataDTO)
    {
        var response = await httpClient.PostAsJsonAsync("api/StockData/save-stock-data", stockDataDTO);
        var data = await response.Content.ReadFromJsonAsync<GeneralResponse>();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(data.Message);
        }

        return data!;        
    }

    public async Task<GeneralResponse> DeleteStockData(int stockDataId)
    {
        var response = await httpClient.DeleteAsync($"api/StockData/delete-stock-data/{stockDataId}");
        var data = await response.Content.ReadFromJsonAsync<GeneralResponse>();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(data.Message);
        }

        return data!; 
    }
}
