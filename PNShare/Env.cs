using System;
using System.Linq;
using System.Net;
using System.Reflection;

namespace PNShare;

public static class Env
{
    public static readonly string AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
    public static readonly string BaseDirectory = AppContext.BaseDirectory;
    public static readonly string CurrentDirectory = Environment.CurrentDirectory;
    public static readonly string Ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString() ?? "127.0.0.1";
}