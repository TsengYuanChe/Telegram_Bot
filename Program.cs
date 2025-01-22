using Telegram.Bot;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// 從配置文件讀取 BotToken
string? botToken = builder.Configuration["BotToken"];
if (string.IsNullOrEmpty(botToken))
{
    throw new ArgumentException("BotToken 未設置於 appsettings.json 中");
}
var botClient = new TelegramBotClient(botToken);

// 註冊服務
builder.Services.AddSingleton(botClient);
builder.Services.AddTransient<WebhookService>();
builder.Services.AddControllers()
       .AddNewtonsoftJson(options =>
           options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

var app = builder.Build();

// 設置 Webhook
using (var scope = app.Services.CreateScope())
{
    var webhookService = scope.ServiceProvider.GetRequiredService<WebhookService>();
    await webhookService.SetWebhookAsync();
}

app.MapControllers();
app.Run();