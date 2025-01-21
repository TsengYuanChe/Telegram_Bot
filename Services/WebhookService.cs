using Telegram.Bot;

public class WebhookService
{
    private readonly TelegramBotClient _botClient;
    private readonly IConfiguration _configuration;

    public WebhookService(TelegramBotClient botClient, IConfiguration configuration)
    {
        _botClient = botClient;
        _configuration = configuration;
    }

    public async Task SetWebhookAsync()
    {
        var webhookUrl = _configuration["WebhookUrl"]; // 從配置中讀取 Webhook URL
        if (string.IsNullOrEmpty(webhookUrl))
        {
            throw new ArgumentException("WebhookUrl 未設置於 appsettings.json 中");
        }

        await _botClient.SetWebhookAsync(webhookUrl);
    }
}