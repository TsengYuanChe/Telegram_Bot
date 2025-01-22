using Telegram.Bot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// 添加日誌服務
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 從配置文件讀取 BotToken
string? botToken = builder.Configuration["BotToken"];
if (string.IsNullOrEmpty(botToken))
{
    throw new ArgumentException("BotToken 未設置於 appsettings.json 中");
}
Console.WriteLine("BotToken 已從配置文件讀取成功");

var botClient = new TelegramBotClient(botToken);

// 註冊服務
builder.Services.AddSingleton(botClient);
builder.Services.AddTransient<WebhookService>();
builder.Services.AddControllers()
       .AddNewtonsoftJson(options =>
           options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

var app = builder.Build();

// 打印啟動環境和 URL 信息
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("應用程序啟動中...");
logger.LogInformation($"環境: {app.Environment.EnvironmentName}");
logger.LogInformation($"服務正在監聽: {app.Urls.FirstOrDefault()}");

app.MapGet("/", () => "應用正在運行");
// 設置 Webhook
using (var scope = app.Services.CreateScope())
{
    var webhookService = scope.ServiceProvider.GetRequiredService<WebhookService>();
    try
    {
        logger.LogInformation("正在設置 Webhook...");
        await webhookService.SetWebhookAsync();
        logger.LogInformation("Webhook 設置成功");
    }
    catch (Exception ex)
    {
        logger.LogError($"設置 Webhook 時出現錯誤: {ex.Message}");
    }
}

// 配置路由
app.MapControllers();

// 啟動應用
logger.LogInformation("應用程序已啟動，正在運行...");
app.Run();