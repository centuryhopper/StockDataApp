using System;
using System.Collections.Generic;

namespace Server.Contexts;

public partial class Stockuser
{
    public int Userid { get; set; }

    public int UmsUserid { get; set; }

    public string Email { get; set; } = null!;

    public DateTime? DateCreated { get; set; }

    public DateTime? DateLastLogin { get; set; }

    public DateTime? DateRetired { get; set; }

    public virtual ICollection<Stockdatum> Stockdata { get; set; } = new List<Stockdatum>();
}
