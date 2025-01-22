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
        // 嘗試讀取原始請求內容
        string rawRequest = string.Empty;
        try
        {
            using (var reader = new StreamReader(HttpContext.Request.Body, leaveOpen: true))
            {
                rawRequest = await reader.ReadToEndAsync();
                HttpContext.Request.Body.Position = 0; // 重設流的位置，確保後續處理不受影響
            }

            if (string.IsNullOrWhiteSpace(rawRequest))
            {
                Console.WriteLine("Received an empty raw request.");
                return BadRequest("Empty request body.");
            }

            Console.WriteLine($"Received raw request: {rawRequest}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading raw request: {ex.Message}");
            return BadRequest("Failed to read request body.");
        }

        // 嘗試解析更新內容
        try
        {
            using var reader = new StreamReader(HttpContext.Request.Body);
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

                try
                {
                    await _botClient.SendMessage(chatId, $"你說了: {messageText}");
                    Console.WriteLine("Message sent successfully.");
                }
                catch (Exception sendEx)
                {
                    Console.WriteLine($"Error sending message: {sendEx.Message}");
                }
            }

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing or processing the update: {ex.Message}");
            return BadRequest("Failed to process update.");
        }
    }
}