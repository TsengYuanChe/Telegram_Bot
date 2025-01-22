using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

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
    public async Task<IActionResult> Post()
    {
        string rawRequest;
        using (var reader = new StreamReader(HttpContext.Request.Body))
        {
            rawRequest = await reader.ReadToEndAsync();
        }
        Console.WriteLine($"Received raw request: {rawRequest}");

        try
        {
            var update = JsonConvert.DeserializeObject<Update>(rawRequest);
            Console.WriteLine($"Parsed update: {update}");

            if (update?.Message?.Text != null)
            {
                var chatId = update.Message.Chat.Id;
                var messageText = update.Message.Text;

                Console.WriteLine($"ChatId: {chatId}, Message received: {messageText}");

                // 回覆消息
                await _botClient.SendMessage(chatId, $"你說了: {messageText}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing update: {ex.Message}");
            return BadRequest("Invalid update format");
        }

        return Ok();
    }
}