using Task_19.TelegramBotScripts.Helpers;
using Task_19.TelegramBotScripts.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace Task_19.TelegramBotScripts.Handlers
{
    public class SortingPreparingHandler : ICommandHandler
    {
        // Manages state of conversation with each user.
        private readonly UserStateManager _userStateManager;

        // Constructor to initialize an instance of the StartCommandHandler class with provided values.
        public SortingPreparingHandler(UserStateManager userStateManager)
        {
            _userStateManager = userStateManager;
        }

        /// <summary>
        /// Asynchronously handles the sorting preparation command, prompting the user to select a field for sorting.
        /// Sets the user state to awaiting sorting input and displays a set of options for sorting fields.
        /// </summary>
        /// <param name="botClient">The Telegram bot client instance to interact with the Telegram API.</param>
        /// <param name="message">The message received from a user that contains the command to be handled.</param>
        /// <param name="cancellationToken">A token that can be used by other objects or threads to receive notice of cancellation, allowing the operation to be cancelled.</param>
        /// <returns>A task representing the asynchronous operation of sending the sorting options message.</returns>
        public async Task HandleCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            // Ensure message is from a user.
            if (message.From == null) return;

            // Sets the user state to indicate they are in the process of selecting a sorting field.
            _userStateManager.SetUserState(message.From.Id, UserState.AwaitingSorting);

            // Creates a reply keyboard with options for sorting fields and additional commands.
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
            {
                // Sort by LibraryName alphabetically.
                new KeyboardButton[] { "/LibraryName" },
                // Sort by CoverageArea in descending order.
                new KeyboardButton[] { "/CoverageArea" },
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

            // Sends a message to the user with the sorting options.
            await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Выберете поле по которому необходимо сделать сортировку.\nLibraryName по алфавиту\nCoverageArea по убыванию", replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);
        }
    }
}