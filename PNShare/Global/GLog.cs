using System.IO;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace PNShare.Global;

public static class GLog
{
    public record Options(string basedir, string dirname, bool writefile, bool writeconsole, params string[] repositorys);

    public static readonly Options DefaultOptions = new Options(Env.BaseDirectory, "__logs", true, true, Env.AssemblyName);

    private static bool _enable;
    private static ILog[] _loggers;

    public static void Init(Options options = default)
    {
        options ??= DefaultOptions;

        if (!options.writeconsole && !options.writefile)
            return;

        _enable = true;

        var di = Directory.CreateDirectory(Path.Combine(options.basedir, options.dirname));
        var pl = new PatternLayout("%d [%t] %-5p %m%n");
        _loggers = new ILog[options.repositorys.Length];
        var i = 0;
        foreach (var repo in options.repositorys)
        {
            var repository = LogManager.CreateRepository(repo);
            repository.Configured = true;

            var hierarchy = (Hierarchy)repository;
            if (options.writeconsole)
            {
                hierarchy.Root.AddAppender(new ConsoleAppender()
                {
                    Name = "Console",
                    Layout = pl
                });
            }
            if (options.writefile)
            {
                var ra = new RollingFileAppender()
                {
                    Name = "RollingFile",
                    AppendToFile = true,
                    DatePattern = "_yyyy_MM_dd_HH'.txt'",
                    StaticLogFileName = false,
                    File = Path.Combine(di.FullName, repo),
                    RollingStyle = RollingFileAppender.RollingMode.Date,
                    Layout = pl
                };
                hierarchy.Root.AddAppender(ra);
                ra.ActivateOptions();
            }
            hierarchy.Root.Level = Level.All;
            _loggers[i++] = LogManager.GetLogger(repo, "Normal");
        }
    }

    public static void Write(object message, int logid = 0)
    {
        if (_enable)
            _loggers[logid].Info(message);
    }
}