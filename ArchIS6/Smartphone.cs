using System;
using System.Collections.Generic;

namespace ArchIS6;

public partial class Smartphone
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Cores { get; set; } = null!;

    public string Price { get; set; } = null!;
}
