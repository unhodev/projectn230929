using System;
using System.IO;
using System.Net.Http;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using PNShare;

namespace TableGenerator;

public static class GDrive
{
    private static DriveService _driveservice;

    public static void Init(string credentialjson = default)
    {
        if (string.IsNullOrEmpty(credentialjson))
            return;

        var credential = GoogleCredential.FromJson(credentialjson).CreateScoped(DriveService.Scope.DriveReadonly);
        var task = credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
        try
        {
            task.Wait();
        }
        catch (Exception e)
        {
            throw new ApplicationException("InvalidCredential", e);
        }

        // 드라이브 서비스 생성
        _driveservice = new DriveService(new BaseClientService.Initializer() { HttpClientInitializer = credential, ApplicationName = Env.AssemblyName });
    }

    public static byte[] GetXlsx(string link)
    {
        return null == _driveservice
            ? GetXlsx_NoAuth(link)
            : GetXlsx_WithAuth(link);
    }

    private static byte[] GetXlsx_WithAuth(string link)
    {
        try
        {
            using var ms = new MemoryStream();
            var request = _driveservice.Files.Export(link, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            request.Download(ms);
            var bytes = ms.ToArray();
            if (bytes.Length == 0)
                throw new ApplicationException("AccessDenied");
            return bytes;
        }
        catch (Exception e)
        {
            throw new ApplicationException(link, e);
        }
    }

    private static byte[] GetXlsx_NoAuth(string link)
    {
        var url = $"https://docs.google.com/spreadsheets/d/{link}/export?format=xlsx";
        using var client = new HttpClient();
        var task = client.GetByteArrayAsync(url);
        try
        {
            task.Wait();
            return task.Result;
        }
        catch (Exception e)
        {
            throw new ApplicationException(link, e);
        }
    }
}