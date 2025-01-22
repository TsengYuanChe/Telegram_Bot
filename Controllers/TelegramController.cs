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
    public async Task<IActionResult> Post()
    {
        // 複製請求流
        using var memoryStream = new MemoryStream();
        await HttpContext.Request.Body.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        // 讀取複製的請求體
        string rawRequest;
        using (var reader = new StreamReader(memoryStream))
        {
            rawRequest = await reader.ReadToEndAsync();
        }

        if (string.IsNullOrWhiteSpace(rawRequest))
        {
            Console.WriteLine("Received an empty raw request.");
            return BadRequest("Empty request body.");
        }

        Console.WriteLine($"Received raw request: {rawRequest}");

        try
        {
            var update = Newtonsoft.Json.JsonConvert.DeserializeObject<Telegram.Bot.Types.Update>(rawRequest);
            if (update == null)
            {
                Console.WriteLine("Failed to parse the update object.");
                return BadRequest("Invalid update format.");
            }

            Console.WriteLine($"Parsed update: {update}");

            if (update.Message?.Text != null)
            {
                var chatId = update.Message.Chat.Id;
                var messageText = update.Message.Text;

                Console.WriteLine($"ChatId: {chatId}, Message: {messageText}");

                await _botClient.SendMessage(chatId, $"你說了: {messageText}");
                Console.WriteLine("Message sent successfully.");
            }

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing update: {ex.Message}");
            return BadRequest("Failed to process update.");
        }
    }
}