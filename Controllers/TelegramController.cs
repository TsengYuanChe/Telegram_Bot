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

            // 確認 message 是否為 null
            if (update.Message == null)
            {
                Console.WriteLine("Message is null");
                return Ok();
            }

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            Console.WriteLine($"ChatId: {chatId}, Message received: {messageText}");

            // 回應訊息
            var reply = $"你說了: {messageText}";
            await _botClient.SendMessage(chatId, reply);
            Console.WriteLine("Message sent successfully");

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing update: {ex.Message}");
            return BadRequest(ex.Message);
        }
    }
}