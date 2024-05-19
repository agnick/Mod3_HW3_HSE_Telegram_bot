using Telegram.Bot;
using Telegram.Bot.Types;

namespace Task_19.TelegramBotScripts.Interfaces
{
    /*
        Defines the interface for command handlers in a Telegram bot application.
        This interface ensures that all command handlers implement a method to process commands asynchronously.
    */
    public interface ICommandHandler
    {
        /// <summary>
        /// Asynchronously handles a command received by the Telegram bot.
        /// </summary>
        /// <param name="botClient">The Telegram bot client instance to interact with the Telegram API.</param>
        /// <param name="message">The message received from a user that contains the command to be handled.</param>
        /// <param name="cancellationToken">A token that can be used by other objects or threads to receive notice of cancellation, allowing the operation to be cancelled.</param>
        /// <returns>A task representing the asynchronous operation, allowing for further actions upon completion.</returns>
        Task HandleCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken);
    }
}
