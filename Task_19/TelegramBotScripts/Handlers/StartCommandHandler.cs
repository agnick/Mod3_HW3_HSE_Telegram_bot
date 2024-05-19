using Task_19.TelegramBotScripts.Helpers;
using Task_19.TelegramBotScripts.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Task_19.TelegramBotScripts.Handlers
{
    public class StartCommandHandler : ICommandHandler
    {
        // Manages state of conversation with each user.
        private readonly UserStateManager _userStateManager;

        // Constructor to initialize an instance of the StartCommandHandler class with provided values.
        public StartCommandHandler(UserStateManager userStateManager)
        {
            _userStateManager = userStateManager;
        }

        /// <summary>
        /// Asynchronously handles the "/start" command, setting the user's state to Initial and sending a welcome message along with initial instructions.
        /// </summary>
        /// <param name="botClient">The Telegram bot client instance to interact with the Telegram API.</param>
        /// <param name="message">The message received from a user that contains the command to be handled.</param>
        /// <param name="cancellationToken">A token that can be used by other objects or threads to receive notice of cancellation, allowing the operation to be cancelled.</param>
        /// <returns>A task representing the asynchronous operation of sending the welcome message.</returns>
        public async Task HandleCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            // Ensure message is from a user.
            if (message.From == null) return;

            // Sets the user's state to Initial upon starting interaction with the bot.
            _userStateManager.SetUserState(message.From.Id, UserState.Initial);

            // // Creates a reply keyboard with options for the user to upload a file or request help.
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
            {
            new KeyboardButton[] { "/uploadFile" },
            new KeyboardButton[] { "/help" },
            })
            {
                // Adjusts keyboard size to fit the screen.
                ResizeKeyboard = true,
                // Keyboard disappears after one use.
                OneTimeKeyboard = true
            };

            // Sends a welcome message to the user along with the keyboard options.
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Добро пожаловать!\nЯ бот, умеющий работать с CSV и JSON файлами типа wifi-library.\nВы можете загрузить файл командой /uploadFile или узнать список доступных команд, используя /help.",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken
            );
        }
    }
}
