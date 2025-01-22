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
        try
        {
            // 打印完整的原始請求
            using var reader = new StreamReader(HttpContext.Request.Body);
            var rawRequest = await reader.ReadToEndAsync();
            Console.WriteLine($"Received raw request: {rawRequest}");

            if (update == null)
            {
                Console.WriteLine("Update is null");
                return BadRequest("Update cannot be null");
            }

            // 處理 Telegram 消息
            if (update.Message?.Text != null)
            {
                var chatId = update.Message.Chat.Id;
                var messageText = update.Message.Text;

                Console.WriteLine($"ChatId: {chatId}, Message: {messageText}");
                await _botClient.SendMessage(chatId, $"你說了: {messageText}");
            }

            return Ok("Request handled successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, "Internal Server Error");
        }
    }
}