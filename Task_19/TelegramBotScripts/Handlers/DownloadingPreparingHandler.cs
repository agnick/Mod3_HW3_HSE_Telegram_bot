using Task_19.TelegramBotScripts.Helpers;
using Task_19.TelegramBotScripts.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace Task_19.TelegramBotScripts.Handlers
{
    public class DownloadingPreparingHandler : ICommandHandler
    {
        // Manages state of conversation with each user.
        private readonly UserStateManager _userStateManager;

        // Constructor to initialize an instance of the FileDownloadHandler class with provided values.
        public DownloadingPreparingHandler(UserStateManager userStateManager)
        {
            _userStateManager = userStateManager;
        }

        /// <summary>
        /// Asynchronously prompts the user to choose the format of the file they wish to download (CSV or JSON).
        /// Sets the user's state to AwaitingDownloading, indicating they are in the process of selecting a download format.
        /// </summary>
        /// <param name="botClient">The Telegram bot client instance to interact with the Telegram API.</param>
        /// <param name="message">The message received from a user, triggering the download preparation.</param>
        /// <param name="cancellationToken">A token that can be used by other objects or threads to receive notice of cancellation, allowing the operation to be cancelled.</param>
        /// <returns>A task representing the asynchronous operation of sending the format selection prompt.</returns>
        public async Task HandleCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            // Ensure the command comes from an actual user.
            if (message.From == null) return;

            // // Update the user's state to indicate they are now choosing a download format.
            _userStateManager.SetUserState(message.From.Id, UserState.AwaitingDownloading);

            // Create a reply keyboard with options for the file formats and additional commands.
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
            {
                // Options for selecting the download format.
                new KeyboardButton[] { "/CSV", "/JSON" },
                // Provides help information.
                new KeyboardButton[] { "/help" },
                // Restarts the bot interaction.
                new KeyboardButton[] { "/start" },
            })
            {
                // Adjusts keyboard size to fit the screen.
                ResizeKeyboard = true,
                // Keyboard disappears after one use.
                OneTimeKeyboard = true
            };

            // Sends a message to the user asking them to choose the download format.
            await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Выберете формат файла для загрузки.", replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);
        }
    }
}