using System;
using System.Collections.Generic;

namespace Server.Contexts;

public partial class Stockdatum
{
    public int Stockdataid { get; set; }

    public int Userid { get; set; }

    public string TickerSymbol { get; set; } = null!;

    public decimal? OpenPrice { get; set; }

    public decimal? ClosePrice { get; set; }

    public decimal? HighPrice { get; set; }

    public decimal? LowPrice { get; set; }

    public DateOnly TradeDate { get; set; }

    public virtual Stockuser User { get; set; } = null!;
}
