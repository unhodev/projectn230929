using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using PNShare;

namespace TableGenerator.Helper;

public static class ExceptionHelper
{
    public static void UnhandledExceptionRegister()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, args) => CreateExceptionFile(
            Directory.CreateDirectory(Path.Combine(Env.BaseDirectory, "_exception")),
            $"{Env.AssemblyName}_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
            (Exception)args.ExceptionObject
        );
        return;

        static void CreateExceptionFile(FileSystemInfo di, string filename, Exception exception)
        {
            using var sw = new StreamWriter(Path.Combine(di.FullName, filename), false, Encoding.UTF8);
            var text = JsonConvert.SerializeObject(exception, Formatting.Indented)
                .Replace(@"\r\n", "\n")
                .Replace(@"\\", @"\");
            sw.Write(text);
            sw.Flush();
            Console.WriteLine($"{filename} Created");
        }
    }
}