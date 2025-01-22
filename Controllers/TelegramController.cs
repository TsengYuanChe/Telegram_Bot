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
        // Debug 輸出原始請求內容
        using var reader = new StreamReader(HttpContext.Request.Body);
        var rawRequest = await reader.ReadToEndAsync();
        Console.WriteLine($"Received raw request: {rawRequest}");

        if (update == null)
        {
            Console.WriteLine("Update is null");
            return BadRequest("Update cannot be null");
        }

        if (update.Type != UpdateType.Message || update.Message == null || update.Message.Text == null)
        {
            Console.WriteLine("Invalid update or no text message received.");
            return Ok();
        }

        var chatId = update.Message.Chat.Id;
        var messageText = update.Message.Text;

        Console.WriteLine($"ChatId: {chatId}, Message: {messageText}");

        try
        {
            var reply = $"你說了: {messageText}";
            var sentMessage = await _botClient.SendMessage(chatId: chatId, text: reply);
            Console.WriteLine($"Message sent successfully: {sentMessage.MessageId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }

        return Ok();
    }
}