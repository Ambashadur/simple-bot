using System;

namespace Simple.Bot;

internal sealed class Objective
{
    public long ChatId { get; set; }

    public string? Name { get; set; }

    public DateTime? DateTime { get; set; }
}
