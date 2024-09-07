using System;
using System.Collections.Generic;

namespace Server.Entities;

public partial class Stockuser
{
    public int Userid { get; set; }

    public string UmsUserid { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime? DateCreated { get; set; }

    public DateTime? DateLastLogin { get; set; }

    public DateTime? DateRetired { get; set; }

    public virtual ICollection<Stockdatum> Stockdata { get; set; } = new List<Stockdatum>();
}
