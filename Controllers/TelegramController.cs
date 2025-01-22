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
        try
        {
            using var reader = new StreamReader(HttpContext.Request.Body);
            var rawRequest = await reader.ReadToEndAsync();
            Console.WriteLine($"Received raw request: {rawRequest}");

            if (update == null)
            {
                Console.WriteLine("Update is null");
                return BadRequest("Update cannot be null");
            }

            Console.WriteLine($"Parsed update: {update}");

            // 如果有消息，處理消息
            if (update.Message?.Text != null)
            {
                Console.WriteLine($"Message received: {update.Message.Text}");
                var chatId = update.Message.Chat.Id;
                var reply = $"你說了: {update.Message.Text}";
                await _botClient.SendMessage(chatId, reply);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing request: {ex.Message}");
        }

        return Ok();
    }
}