using Task_19.TelegramBotScripts.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace Task_19.TelegramBotScripts.Handlers
{
    public class HelpCommandHandler : ICommandHandler
    {
        /// <summary>
        /// Asynchronously sends a help message detailing the available bot commands and their purposes.
        /// This method allows users to understand how to interact with the bot and the functionalities it offers.
        /// </summary>
        /// <param name="botClient">The Telegram bot client instance to interact with the Telegram API.</param>
        /// <param name="message">The message received from a user that contains the "/help" command.</param>
        /// <param name="cancellationToken">A token that can be used by other objects or threads to receive notice of cancellation, allowing the operation to be cancelled.</param>
        /// <returns>A task representing the asynchronous operation of sending the help message.</returns>
        public async Task HandleCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            // Constructs and sends a message detailing the commands available in the bot.
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Вы можете использовать следующие команды:\n" +
                      "/uploadFile - загрузить CSV или JSON файл, соответствующий варианту\n" +
                      "/sort - выборка по полю в файле\n" +
                      "/select - сортировка по полю в файле\n" +
                      "/download - скачать обработанный файл\n" +
                      "/help - узнать список команд\n" +
                      "/start - перезапустить бота",
                cancellationToken: cancellationToken
            );
        }
    }
}
