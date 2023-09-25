using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PNShare.Global;

GConfig.Init();
GLog.Init(GLog.DefaultOptions with { writefile = GConfig.On("GLog.WriteFile"), writeconsole = GConfig.On("GLog.WriteConsole") });

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();