using Shared.Models;
using static Shared.Models.ServiceResponses;

namespace Client.Interfaces;

public interface IStockDataService
{
    Task<IEnumerable<StockDataDTO>> GetStockData();
    Task<StockRealTimeInfo?> GetStockRealTimeData(string tickerSymbol);
    Task<HistoricalStockData?> GetStockHistoricalData(string tickerSymbol);
    Task<GeneralResponse> SaveStockData(StockDataDTO stockDataDTO);
    Task<GeneralResponse> UpdateStockData(string tickerSymbol, StockRealTimeInfo stockRealTimeInfo);
    Task<GeneralResponse> DeleteStockData(int stockDataId);
}


