using PNShare.DB;
using PNUnity.Share;

namespace PNGame.Modules;

public class ModuleError
{
    public static ModuleError SUCCESS => default;

    public ErrorCode code;
    public string message;
    public string filepath;
    public int line;
}

public static partial class GModule
{
    private static ModuleError ToModuleError(this ErrorCode ec, string filepath, int line, string message = default) => new ModuleError()
    {
        code = ec,
        message = message ?? ec.ToString(),
        filepath = filepath,
        line = line,
    };

    private static ModuleError ToModuleError(this MongoResult mr, string filepath, int line, string message = default) => new ModuleError()
    {
        code = ErrorCode.DB_ERROR,
        message = message ?? mr.ToString(),
        filepath = filepath,
        line = line,
    };
}