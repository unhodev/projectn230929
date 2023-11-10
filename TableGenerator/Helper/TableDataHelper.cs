using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;

namespace TableGenerator.Helper;

public static class TableDataHelper
{
    private static readonly Regex _REGEX_BLANK = new Regex("\\s+");

    public static bool IgnoreTableName(string name) => name.StartsWith('#');

    public static List<List<string>> DataFilter(List<List<string>> origin)
    {
        return origin
            .Select(TrimValues)
            .Where(NoEmptyRow)
            .ToList();

        static List<string> TrimValues(List<string> row) => row
            .Select(x => _REGEX_BLANK
                .Replace(x, "")
                .Split('#')
                .FirstOrDefault()
            ).ToList();

        static bool NoEmptyRow(List<string> row) => row.Any(x => !string.IsNullOrEmpty(x));
    }

    public static bool TryGetError(List<List<string>> datas, out string errmsg)
    {
        if (datas.Count <= 2)
        {
            errmsg = "rows <= 2";
            return false;
        }

        errmsg = string.Empty;
        return true;
    }

    public static void WriteFile(string filename, List<List<string>> datas)
    {
        using var sw = new StreamWriter(filename, false, Encoding.UTF8);
        using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
        foreach (var row in datas)
        {
            row.ForEach(x => csv.WriteField(x));
            csv.NextRecord();
        }
    }
}