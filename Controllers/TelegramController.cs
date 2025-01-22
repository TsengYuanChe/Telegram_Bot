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
            // 檢查 update 是否為空
            if (update == null)
            {
                Console.WriteLine("Update is null");
                return BadRequest("Update cannot be null");
            }

            // 檢查是否為有效的消息
            if (update.Type == UpdateType.Message && update.Message?.Text != null)
            {
                var chatId = update.Message.Chat.Id;
                var messageText = update.Message.Text;

                Console.WriteLine($"ChatId: {chatId}, Message: {messageText}");

                // 回覆消息
                await _botClient.SendMessage(chatId, $"你說了: {messageText}");
            }

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing update: {ex.Message}");
            return StatusCode(500, "Internal Server Error");
        }
    }
}