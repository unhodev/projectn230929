using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        var env = GetPublicStaticFieldsAsDictionary(typeof(Env));

        var merge = origin.Union(env)
            .GroupBy(x => x.Key)
            .ToDictionary(k => k.Key, v => v.First().Value);

        _map1 = ResolveKeys(merge);

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

        static Dictionary<string, string> GetPublicStaticFieldsAsDictionary(Type type)
        {
            var fieldDictionary = new Dictionary<string, string>();

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
                fieldDictionary[$"{type.Name}.{field.Name}"] = field.GetValue(null)?.ToString();

            return fieldDictionary;
        }
    }

    public static string Get(string key) => _map1.GetValueOrDefault(key);
    public static int GetInt(string key) => _map1.TryGetValue(key, out var value) ? int.Parse(value) : default;
    public static bool On(string key) => Get(key) == "ON";

    public static DbOptions GetDbOptions(string section)
    {
        var options = new DbOptions();
        options.IsAtlas = On($"{section}.{nameof(options.IsAtlas)}");
        options.Address = Get($"{section}.{nameof(options.Address)}");
        options.Username = Get($"{section}.{nameof(options.Username)}");
        options.Password = Get($"{section}.{nameof(options.Password)}");
        options.DBName = Get($"{section}.{nameof(options.DBName)}");
        options.TimeoutMs = GetInt($"{section}.{nameof(options.TimeoutMs)}");
        options.MaxPoolSize = GetInt($"{section}.{nameof(options.MaxPoolSize)}");
        options.AutoMigration = On($"{section}.{nameof(options.AutoMigration)}");
        return options;
    }

    public static Dictionary<string, string> GetDict(string key)
    {
        var key1 = $"{key}.";
        return _map1
            .Where(x => x.Key.StartsWith(key1))
            .ToDictionary(k => k.Key[key1.Length..], v => v.Value);
    }
}

public class DbOptions
{
    public bool IsAtlas;
    public string Address;
    public string Username;
    public string Password;
    public string DBName;
    public int TimeoutMs;
    public int MaxPoolSize;
    public bool AutoMigration;
}