using Task_19.TelegramBotScripts.Helpers;
using Telegram.Bot.Types;
using Telegram.Bot;
using Task_19.TelegramBotScripts.Interfaces;

namespace Task_19.TelegramBotScripts.Handlers
{
    public class SelectionPickedHandler : ICommandHandler
    {
        // Manages state of conversation with each user.
        private readonly UserStateManager _userStateManager;
        // The specific field chosen for filtering.
        private readonly string _field;

        // Constructor to initialize an instance of the SelectionPickedHandler class with provided values.
        public SelectionPickedHandler(UserStateManager userStateManager, string field)
        {
            _userStateManager = userStateManager;
            _field = field;
        }

        /// <summary>
        /// Asynchronously handles the event when a user picks a selection for filtering.
        /// It updates the user's state to awaiting input for the chosen field's value and prompts the user to enter the value(s).
        /// </summary>
        /// <param name="botClient">The Telegram bot client instance to interact with the Telegram API.</param>
        /// <param name="message">The message received from a user that contains the selection.</param>
        /// <param name="cancellationToken">A token that can be used by other objects or threads to receive notice of cancellation, allowing the operation to be cancelled.</param>
        /// <returns>A task representing the asynchronous operation of sending a prompt for field value input.</returns>
        public async Task HandleCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            // Ensure message is from a user.
            if (message.From == null) return;

            // Updates the user's state to indicate they are now expected to provide a value for the chosen field.
            _userStateManager.SetUserState(message.From.Id, UserState.AwaitingFieldValue);
            // Stores the selected field in the user's session for later use during data filtering.
            _userStateManager.SetUserField(message.From.Id, _field);

            // Sends a message to the user, prompting them to input the value(s) for the chosen field.
            // If the selection is based on multiple fields, instructs to separate values with a semicolon.
            await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Введите значение поля {_field} для организации выборки.\nЕсли выборка осуществляется по двум полям, то введите значения полей через точку-запятую без пробелов.\nПример ввода: действует;открытая сеть", cancellationToken: cancellationToken);
        }
    }
}