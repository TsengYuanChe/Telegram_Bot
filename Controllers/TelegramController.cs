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
            // 反序列化 JSON
            var update = JsonConvert.DeserializeObject<Update>(rawRequest, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                Error = (sender, args) =>
                {
                    Console.WriteLine($"Deserialization error: {args.ErrorContext.Error.Message}");
                    args.ErrorContext.Handled = true;
                }
            });

            if (update == null)
            {
                Console.WriteLine("Failed to parse update: Deserialized object is null");
                return BadRequest("Invalid update format");
            }

            Console.WriteLine($"Parsed update: {JsonConvert.SerializeObject(update, Formatting.Indented)}");

            // 處理接收到的訊息
            if (update.Message?.Text != null)
            {
                var chatId = update.Message.Chat.Id;
                var messageText = update.Message.Text;

                Console.WriteLine($"ChatId: {chatId}, Message: {messageText}");

                // 回覆訊息
                var reply = $"你說了: {messageText}";
                await _botClient.SendMessage(chatId, reply);
                Console.WriteLine("Message sent successfully");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing update: {ex.Message}");
            return BadRequest("Error processing update");
        }

        return Ok();
    }
}