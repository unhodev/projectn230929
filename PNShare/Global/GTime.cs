using System;

namespace PNShare.Global;

public static class GTime
{
    public record Options(int addseconds);

    public static Options DefaultOptions = new Options(0);

    private static int _addseconds;

    public static void Init(Options options = default)
    {
        options ??= DefaultOptions;
        _addseconds = options.addseconds;
    }

    public static DateTime Now() => DateTime.UtcNow.AddSeconds(_addseconds);
}