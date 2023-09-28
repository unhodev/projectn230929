using Newtonsoft.Json;

namespace PNShare.Global;

public static class GJson
{
    public static string SerializeObject(object value) => JsonConvert.SerializeObject(value);
}