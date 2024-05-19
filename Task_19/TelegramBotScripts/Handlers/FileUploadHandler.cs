using Task_19.TelegramBotScripts.Helpers;
using Task_19.TelegramBotScripts.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Task_19.TelegramBotScripts.Handlers
{
    public class FileUploadHandler : ICommandHandler
    {
        // Manages state of conversation with each user.
        private readonly UserStateManager _userStateManager;

        // Constructor to initialize an instance of the FileUploadHandler class with provided values.
        public FileUploadHandler(UserStateManager userStateManager)
        {
            _userStateManager = userStateManager;
        }

        /// <summary>
        /// Asynchronously prompts the user to upload a file by setting their state to AwaitingFileUpload and sending an instruction message.
        /// This method facilitates the process of receiving files from users, indicating the types of files accepted.
        /// </summary>
        /// <param name="botClient">The Telegram bot client instance to interact with the Telegram API.</param>
        /// <param name="message">The message received from a user, triggering the file upload process.</param>
        /// <param name="cancellationToken">A token that can be used by other objects or threads to receive notice of cancellation, allowing the operation to be cancelled.</param>
        /// <returns>A task representing the asynchronous operation of sending the file upload prompt.</returns>
        public async Task HandleCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            // Ensure message is from a user.
            if (message.From == null) return;

            // Sets the user's state to indicate they are expected to upload a file next.
            _userStateManager.SetUserState(message.From.Id, UserState.AwaitingFileUpload);

            // Sends a message to the user, asking them to upload a CSV or JSON file.
            await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Загрузите CSV или JSON файл.", cancellationToken: cancellationToken);
        }
    }
}
