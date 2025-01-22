using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.IO;

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
        // 記錄原始請求
        using var reader = new StreamReader(HttpContext.Request.Body);
        var rawRequest = await reader.ReadToEndAsync();
        Console.WriteLine($"Raw request: {rawRequest}");

        if (update == null)
        {
            Console.WriteLine("Update is null");
            return BadRequest("Update cannot be null");
        }

        Console.WriteLine($"Received update: {update}");

        if (update.Type != UpdateType.Message || update.Message == null)
        {
            Console.WriteLine("Update is not a message or message is null");
            return Ok();
        }

        // 確保 message 和 chat 資料存在
        if (update.Message.Chat == null || update.Message.Text == null)
        {
            Console.WriteLine("Invalid message data");
            return Ok();
        }

        var chatId = update.Message.Chat.Id;
        var messageText = update.Message.Text;

        Console.WriteLine($"ChatId: {chatId}");
        Console.WriteLine($"MessageText: {messageText}");

        try
        {
            var reply = $"你說了: {messageText}";
            await _botClient.SendMessage(chatId, reply); // 使用正確的 Telegram API 方法
            Console.WriteLine("Message sent successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }

        return Ok();
    }
}