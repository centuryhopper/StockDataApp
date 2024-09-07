using System;
using System.Collections.Generic;

namespace Server.Entities;

public partial class Stockdatum
{
    public int Stockdataid { get; set; }

    public int Userid { get; set; }

    public string TickerSymbol { get; set; } = null!;

    public decimal? OpenPrice { get; set; }

    public decimal? ClosePrice { get; set; }

    public decimal? HighPrice { get; set; }

    public decimal? LowPrice { get; set; }

    public DateOnly DateCreated { get; set; }

    public decimal? CurrentPrice { get; set; }

    public decimal? Delta { get; set; }

    public decimal? PercentDelta { get; set; }

    public decimal? PreviousClose { get; set; }

    public virtual Stockuser User { get; set; } = null!;
}
