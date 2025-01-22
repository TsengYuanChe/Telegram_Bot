using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

[ApiController]
[Route("api/telegram")]
public class TelegramController : ControllerBase
{
    private readonly TelegramBotClient _botClient;

    public TelegramController(TelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        // 啟用請求 Body 緩衝
        HttpContext.Request.EnableBuffering();

        // 記錄原始請求
        using (var reader = new StreamReader(HttpContext.Request.Body, leaveOpen: true))
        {
            var rawRequest = await reader.ReadToEndAsync();
            Console.WriteLine($"Received raw request: {rawRequest}");
            HttpContext.Request.Body.Position = 0; // 重置流位置
        }

        if (update == null)
        {
            Console.WriteLine("Update is null");
            return BadRequest("Update cannot be null");
        }

        Console.WriteLine($"Parsed update: {update}");

        if (update.Type != UpdateType.Message || update.Message == null)
        {
            Console.WriteLine("Update is not a message or message is null");
            return Ok();
        }

        var chatId = update.Message.Chat.Id;
        var messageText = update.Message.Text;

        Console.WriteLine($"ChatId: {chatId}");
        Console.WriteLine($"Message received: {messageText}");

        try
        {
            var reply = $"你說了: {messageText}";
            await _botClient.SendMessage(chatId, reply); // 使用正確的 SendMessage 方法
            Console.WriteLine("Message sent successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }

        return Ok();
    }
}