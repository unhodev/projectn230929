using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace PNShare.Global;

public static class GConfig
{
    public record Options(string filename, string dirpath);

    private static readonly Options DefaultOptions = new Options($"{Env.AssemblyName}.ini", Env.BaseDirectory);

    private static Dictionary<string, string> _map1;

    public static void Init(Options options = default)
    {
        options ??= DefaultOptions;

        var origin = new ConfigurationManager()
            .SetBasePath(options.dirpath)
            .AddIniFile(options.filename)
            .Build()
            .AsEnumerable()
            .Where(x => !string.IsNullOrEmpty(x.Value) && !string.IsNullOrEmpty(x.Key))
            .ToDictionary(k => k.Key.Replace(':', '.'), v => v.Value);

        _map1 = ResolveKeys(origin);

        return;

        static Dictionary<string, string> ResolveKeys(Dictionary<string, string> input)
        {
            var resolved = new Dictionary<string, string>();

            bool hasUnresolvedValue;
            do
            {
                hasUnresolvedValue = false;

                foreach (var entry in input)
                {
                    var resolvedValue = ResolveValue(input, entry.Value);
                    if (entry.Value != resolvedValue)
                        hasUnresolvedValue = true;

                    input[entry.Key] = resolvedValue;
                }
            } while (hasUnresolvedValue);

            foreach (var entry in input)
            {
                resolved[entry.Key] = entry.Value;
            }

            return resolved;
        }

        static string ResolveValue(IReadOnlyDictionary<string, string> dictionary, string value)
        {
            var regex = new Regex(@"{([\w|.]+)}");
            var matches = regex.Matches(value);
            foreach (Match match in matches)
            {
                var key = match.Groups[1].Value;
                if (!dictionary.ContainsKey(key))
                    continue;

                var replacement = dictionary[key];
                value = value.Replace($"{{{key}}}", replacement);
            }
            return value;
        }
    }

    public static string Get(string key) => _map1.GetValueOrDefault(key);
    public static bool On(string key) => Get(key) == "ON";
}