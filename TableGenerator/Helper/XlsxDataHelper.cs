using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ExcelDataReader;

namespace TableGenerator.Helper;

public static class XlsxDataHelper
{
    static XlsxDataHelper()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public static Dictionary<string, List<List<string>>> Read(string filepath)
    {
        var map = new Dictionary<string, List<List<string>>>();
        using var stream = File.Open(filepath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);
        using var ds = reader.AsDataSet();
        foreach (DataTable datatable in ds.Tables)
        {
            var rows = datatable.Rows.OfType<DataRow>()
                .Select(x => x.ItemArray.Select(y => y?.ToString() ?? string.Empty).ToList())
                .ToList();
            map.Add(datatable.TableName, rows);
        }

        return map;
    }
}