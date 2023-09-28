using System;

namespace PNShare.Func;

public static class FuncPlayer
{
    public static DateTime NewTokenExpire(DateTime now) => now.AddHours(1);
}