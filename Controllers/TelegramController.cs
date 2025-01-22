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
        if (update == null) 
        {
            Console.WriteLine("Update is null");
            return BadRequest("Update cannot be null");
        };
        
        Console.WriteLine($"Received update: {update}");
        Console.WriteLine($"ChatId: {update.Message?.Chat.Id}");
        Console.WriteLine($"MessageText: {update.Message?.Text}");


        if (update.Message?.Text != null)
        {
            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            var reply = $"你說了: {messageText}";
            await _botClient.SendMessage(chatId, reply);
        }

        return Ok();
    }
}