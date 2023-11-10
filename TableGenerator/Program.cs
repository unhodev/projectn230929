using System;
using System.IO;
using PNShare.Global;
using TableGenerator;
using TableGenerator.Helper;

ExceptionHelper.UnhandledExceptionRegister();

GConfig.Init();
GDrive.Init(GConfig.Get("Google.Credential"));

var sourcemap = GConfig.GetDict("Source");
var xlsxdir = GConfig.Get("Directory.XLSX") ?? ".";
Directory.CreateDirectory(xlsxdir);

// 1. 구글시트 -> xlsx
foreach (var kv in sourcemap)
{
    var bytes = GDrive.GetXlsx(kv.Value);
    var filepath = $"{xlsxdir}/{kv.Key}.xlsx";
    File.WriteAllBytes(filepath, bytes);
    Console.WriteLine(filepath);
}

// 2. xlsx -> csv
var csvdir = GConfig.Get("Directory.CSV") ?? ".";
Directory.CreateDirectory(csvdir);

foreach (var kv in sourcemap)
{
    var filepath1 = $"{xlsxdir}/{kv.Key}.xlsx";
    var dics = XlsxDataHelper.Read(filepath1);
    foreach (var kv2 in dics)
    {
        if (TableDataHelper.IgnoreTableName(kv2.Key))
            continue;

        var filepath2 = $"{csvdir}/{kv2.Key}.csv";
        var datas = TableDataHelper.DataFilter(kv2.Value);
        if (TableDataHelper.TryGetError(datas, out var errmsg))
            throw new ApplicationException($"{kv.Key}.{kv2.Key}: {errmsg}");
        TableDataHelper.WriteFile(filepath2, datas);
    }
}

Console.WriteLine("Complete");