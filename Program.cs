using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecondBot
{
    class Program
    {
        private static readonly string BotToken2 = "BotToken"; 
        private static readonly string ChannelLink = "ChannelLink"; 

        private static Dictionary<string, string> codeFileIds = new Dictionary<string, string>();

        static async Task Main(string[] args)
        {
            var botClient2 = new TelegramBotClient(BotToken2);
            var cts = new CancellationTokenSource();

            try
            {
              
                botClient2.StartReceiving(
                    HandleUpdateAsync,  
                    HandleErrorAsync,   
                    cancellationToken: cts.Token
                );

                Console.WriteLine("bot ishlamoqda...");
                Console.ReadLine(); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xatolik: {ex.Message}");
            }
        }

        static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message != null)
            {
                var message = update.Message;

                if (message.Text?.StartsWith("/start") == true)
                {
                    
                    var chatId = message.Chat.Id;
                    var member = await botClient.GetChatMemberAsync(chatId, message.From.Id);

                    if (member.Status == ChatMemberStatus.Member || member.Status == ChatMemberStatus.Administrator)
                    {
                      
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Kino qodini kiriting",
                            cancellationToken: cancellationToken
                        );
                    }
                    else
                    {
                       
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: $"Iltimos, kanalga qo'shiling va qaytib Start tugmasini bosing! Kanalga qo'shilish uchun: {ChannelLink}",
                            cancellationToken: cancellationToken
                        );
                    }
                }
                else if (message.Text != null && int.TryParse(message.Text, out _))
                {
                    string code = message.Text;

                 
                    if (codeFileIds.ContainsKey(code))
                    {
                        var fileId = codeFileIds[code];

                       
                        await botClient.SendDocumentAsync(
                            chatId: message.Chat.Id,
                            document: fileId,
                            caption: "Siz so'ragan fayl",
                            cancellationToken: cancellationToken
                        );
                    }
                    else
                    {
                        
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Bunday kod topilmadi! Iltimos, to'g'ri kod kiriting.",
                            cancellationToken: cancellationToken
                        );
                    }
                }
            }
        }

        static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API xatosi:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }
    }
}
