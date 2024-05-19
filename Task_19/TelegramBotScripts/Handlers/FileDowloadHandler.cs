using Task_19.TelegramBotScripts.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot;
using Task_19.TelegramBotScripts.Helpers;
using DataProcessing;
using Microsoft.Extensions.Logging;

namespace Task_19.TelegramBotScripts.Handlers
{
    public class FileDownloadHandler : ICommandHandler
    {
        // Manages state of conversation with each user.
        private readonly UserStateManager _userStateManager;
        // File extension name. 
        private string _exName;
        // Allows logging of messages and errors to a file and to the console.
        private readonly ILoggerFactory _loggerFactory;

        // Constructor to initialize an instance of the FileDownloadHandler class with provided values.
        public FileDownloadHandler(UserStateManager userStateManager, ILoggerFactory loggerFactory, string exName)
        {
            _userStateManager = userStateManager;
            _loggerFactory = loggerFactory;
            _exName = exName;
        }

        /// <summary>
        /// Asynchronously processes a request to download the processed file in the specified format.
        /// </summary>
        /// <param name="botClient">The Telegram bot client instance to interact with the Telegram API.</param>
        /// <param name="message">The message received from a user, triggering the file download.</param>
        /// <param name="cancellationToken">A token that can be used by other objects or threads to receive notice of cancellation, allowing the operation to be cancelled.</param>
        /// <returns>A task representing the asynchronous operation of sending the processed file to the user.</returns>
        public async Task HandleCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            // Ensure message is from a user.
            if (message.From == null) return;

            // Retrieves the user's processed data.
            List<WifiLibrary>? wifiLibraries = _userStateManager.GetUserLibraries(message.From.Id);

            // Logger initialization.
            ILogger logger = _loggerFactory.CreateLogger("TgBot");

            // Checks if there is data available for downloading.
            if (wifiLibraries == null)
            {
                // Outputting a message to a file and to the console.
                logger.LogError("Коллекция не найдена.");
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Коллекция не найдена.", cancellationToken: cancellationToken);

                return;
            }

            await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Подготовка файла к скачиванию...", cancellationToken: cancellationToken);

            try
            {
                // Processes and sends the file in the requested format.
                if (_exName == "CSV")
                {
                    CSVProcessing csvProcessing = new CSVProcessing();
                    using (Stream streamWrite = csvProcessing.Write(wifiLibraries))
                    {
                        // Resets the stream's position for reading.
                        streamWrite.Position = 0;

                        // Generates a unique file name.
                        string newFileName = $"wifi-library_{DateTime.Now:yyyyMMddHHmmss}.csv";

                        // Sends the CSV file to the user.
                        await botClient.SendDocumentAsync(
                            chatId: message.Chat.Id,
                            document: InputFile.FromStream(stream: streamWrite, fileName: newFileName),
                            caption: "Ваш файл готов к скачиванию",
                            cancellationToken: cancellationToken
                        );
                    }
                }
                // Handles JSON file format.
                else
                {
                    JSONProcessing jsonProcessing = new JSONProcessing();
                    using (Stream streamWrite = jsonProcessing.Write(wifiLibraries))
                    {
                        // Resets the stream's position for reading.
                        streamWrite.Position = 0;

                        // Generates a unique file name.
                        string newFileName = $"wifi-library_{DateTime.Now:yyyyMMddHHmmss}.json";

                        // Sends the JSON file to the user.
                        await botClient.SendDocumentAsync(
                            chatId: message.Chat.Id,
                            document: InputFile.FromStream(stream: streamWrite, fileName: newFileName),
                            caption: "Ваш файл готов к скачиванию",
                            cancellationToken: cancellationToken
                        );
                    }
                }
            }
            // Catches and logs any errors that occur during the file preparation or sending process.
            catch (Exception ex)
            {
                // Outputting a message to a file and to the console.
                logger.LogError($"{ex.Message}\nПроизошла ошибка при подготовке файла.");
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"{ex.Message}\nПроизошла ошибка при подготовке файла.", cancellationToken: cancellationToken);
            }

            // Informs the user on how to restart the bot for a new session.
            await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Для перезапуска бота используйте команду /start", cancellationToken: cancellationToken);
        }
    }
}