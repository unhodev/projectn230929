using System;
using System.IO;
using PNShare.Global;
using TableGenerator;

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

Console.WriteLine("Complete");