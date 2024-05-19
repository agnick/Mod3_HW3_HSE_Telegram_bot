using Task_19.TelegramBotScripts.Helpers;
using Task_19.TelegramBotScripts.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Task_19.TelegramBotScripts.Handlers
{
    public class SelectionPreparingHandler : ICommandHandler
    {
        // Manages state of conversation with each user.
        private readonly UserStateManager _userStateManager;

        // Constructor to initialize an instance of the SelectionPreparingHandler class with provided values.
        public SelectionPreparingHandler(UserStateManager userStateManager)
        {
            _userStateManager = userStateManager;
        }

        /// <summary>
        /// Asynchronously handles the selection preparation command, setting the user's state to awaiting selection and presenting them with field options for making a data selection.
        /// </summary>
        /// <param name="botClient">The Telegram bot client instance to interact with the Telegram API.</param>
        /// <param name="message">The message received from a user that contains the command to be handled.</param>
        /// <param name="cancellationToken">A token that can be used by other objects or threads to receive notice of cancellation, allowing the operation to be cancelled.</param>
        /// <returns>A task representing the asynchronous operation of sending the selection options message.</returns>
        public async Task HandleCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            // Ensure message is from a user.
            if (message.From == null) return;

            // Update the user's state to indicate they are now in the selection phase.
            _userStateManager.SetUserState(message.From.Id, UserState.AwaitingSelection);

            // Create a reply keyboard with options for the fields that can be selected for data querying.
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
            {
                // Options for selecting data by administrative area or WiFi name.
                new KeyboardButton[] { "/AdmArea", "/WiFiName" },
                // Option for selecting data by functionality and access flags.
                new KeyboardButton[] { "/FunctionFlagAndAccessFlag" },
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

            // Sends a message to the user with options for making a data selection.
            await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Выберете поле по которому необходимо сделать выборку.", replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);
        }
    }
}
