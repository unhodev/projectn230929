using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PNShare.DB;
using PNShare.Global;

GConfig.Init();
GTime.Now();
GLog.Init(GLog.DefaultOptions with { writefile = GConfig.On("GLog.WriteFile"), writeconsole = GConfig.On("GLog.WriteConsole") });
GameDB.Init(GConfig.GetDbOptions(nameof(GameDB)));

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();