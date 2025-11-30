using RequestMetrics.Extensions;
using Serilog;
using TestWebApp.Attributes;

var builder = WebApplication.CreateBuilder(args);
//Directory.SetCurrentDirectory(AppContext.BaseDirectory);
//Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));
//#region config Serilog using Another setting json file
//builder.Configuration.AddJsonFile("SerilogSetting.json", optional: false, reloadOnChange: true);
//builder.Host.UseSerilog((ctx, lc) =>
//{
//    lc.ReadFrom.Configuration(ctx.Configuration);
//});
//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)
//    .CreateLogger();
////#region For signalR Approach
////builder.Services.AddSignalR(options =>
////{
////    options.AddFilter<SerilogHubFilter>();
////});
////#endregion
//#endregion

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new LogExecutionTimeAttribute());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseMySerilog();

builder.Services.AddSystemMetrics();
//builder.Services.AddHostedService<SystemMetricsWorker>();

var app = builder.Build();

app.UseRequestMetrics();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//Log.Information("Hello from Serilog Console!");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGet("/GetHello", () => "Hello");
app.Run();
