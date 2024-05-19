using Task_19.TelegramBotScripts.Helpers;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.Logging;

namespace Task_19.TelegramBotScripts.Handlers
{
    public class FileReceivedHandler
    {
        // Manages state of conversation with each user.
        private readonly UserStateManager _userStateManager;
        // Manages file type settings with each user.
        private readonly FileTypeManager _fileTypeManager;
        // Allows logging of messages and errors to a file and to the console.
        private readonly ILoggerFactory _loggerFactory;

        // Constructor to initialize an instance of the FileReceivedHandler class with provided values.
        public FileReceivedHandler(UserStateManager userStateManager, FileTypeManager fileTypeManager, ILoggerFactory loggerFactory)
        {
            _userStateManager = userStateManager;
            _fileTypeManager = fileTypeManager;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Asynchronously processes an uploaded file, checking its type and saving it for further actions.
        /// If the file is not of a supported format (CSV or JSON), a warning is sent to the user.
        /// </summary>
        /// <param name="botClient">The Telegram bot client instance to interact with the Telegram API.</param>
        /// <param name="message">The message containing the document uploaded by the user.</param>
        /// <param name="cancellationToken">A token for canceling the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task HandleFileAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            // Checks for the validity of the message and the presence of a document.
            if (message.From == null || message.Document == null || message.Document.FileName == null) return;

            string fileName = message.Document.FileName.ToLower();

            // Ensures the file is either a CSV or JSON.
            if (!fileName.EndsWith(".csv") && !fileName.EndsWith(".json"))
            {
                // Logger initialization.
                ILogger logger = _loggerFactory.CreateLogger("TgBot");
                // Outputting a message to a file and to the console.
                logger.LogError("Пожалуйста, загрузите файл в формате CSV или JSON.");
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Пожалуйста, загрузите файл в формате CSV или JSON.", cancellationToken: cancellationToken);
                return;
            }

            // Downloads the file from Telegram servers.
            var file = await botClient.GetFileAsync(message.Document.FileId, cancellationToken);
            if (file.FilePath == null) return;

            // Saves the file to a stream for further processing.
            Stream fileStream = new MemoryStream();
            await botClient.DownloadFileAsync(file.FilePath, fileStream, cancellationToken);
            // Resets the stream's position for reading.
            fileStream.Position = 0;

            // Updates the user's state and file in the system.
            _userStateManager.SetUserFile(message.From.Id, fileStream);
            _fileTypeManager.SetFileType(message.From.Id, fileName.EndsWith(".csv") ? FileType.CSV : FileType.JSON);
            _userStateManager.SetUserState(message.From.Id, UserState.FileUploaded);

            // Provides the user with the next steps after successful file upload.
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "/select", "/sort" },
                new KeyboardButton[] { "/help" },
                new KeyboardButton[] { "/start" },
            })
            {
                // Adjusts keyboard size to fit the screen.
                ResizeKeyboard = true,
                // Keyboard disappears after one use.
                OneTimeKeyboard = true
            };

            // Notifies the user that the file has been successfully uploaded and presents possible next actions.
            await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Файл успешно загружен, выберете следующее действие.", replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);
        }
    }
}
