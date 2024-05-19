using Task_19.TelegramBotScripts.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot;
using Task_19.TelegramBotScripts.Helpers;
using DataProcessing;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.Logging;

namespace Task_19.TelegramBotScripts.Handlers
{
    public class DataSortingHandler : ICommandHandler
    {
        // Manages state of conversation with each user.
        private readonly UserStateManager _userStateManager;
        // Manages file type settings with each user.
        private readonly FileTypeManager _fileTypeManager;
        // Allows logging of messages and errors to a file and to the console.
        private readonly ILoggerFactory _loggerFactory;
        // Field to sort by.
        private readonly string _field;

        // Constructor to initialize an instance of the DataSortingHandler class with provided values.
        public DataSortingHandler(UserStateManager userStateManager, FileTypeManager fileTypeManager, ILoggerFactory loggerFactory, string field)
        {
            _userStateManager = userStateManager;
            _fileTypeManager = fileTypeManager;
            _loggerFactory = loggerFactory;
            _field = field;
        }

        /// <summary>
        /// Processes the sorting command by reading the uploaded file, sorting the data based on the specified field, and updating the user's state.
        /// Informs the user about the outcome of the operation and next steps.
        /// </summary>
        /// <param name="botClient">The Telegram bot client instance for communication.</param>
        /// <param name="message">The message triggering this command.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task HandleCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            // Ensure the command comes from an actual user.
            if (message.From == null) return;

            // Logger initialization.
            ILogger logger = _loggerFactory.CreateLogger("TgBot");

            Stream? userFile = _userStateManager.GetUserFile(message.From.Id);

            if (userFile == null)
            {              
                // Outputting a message to a file and to the console.
                logger.LogError("Файл не найден. Пожалуйста, загрузите файл.");
                // Inform the user if no file was found.
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Файл не найден. Пожалуйста, загрузите файл.", cancellationToken: cancellationToken);
                
                return;
            }

            try
            {
                List<WifiLibrary>? wifiLibraries;

                // Read the file based on the user's selected file type.
                if (_fileTypeManager.GetFileType(message.From.Id) == FileType.CSV)
                {
                    CSVProcessing csvProcessing = new CSVProcessing();
                    wifiLibraries = csvProcessing.Read(userFile);
                }
                else
                {
                    JSONProcessing jSONProcessing = new JSONProcessing();
                    wifiLibraries = jSONProcessing.Read(userFile);
                }

                // Handle file reading errors.
                if (wifiLibraries == null) 
                {
                    // Outputting a message to a file and to the console.
                    logger.LogError("Ошибка чтения файла.");
                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Ошибка чтения файла.", cancellationToken: cancellationToken);

                    return;
                }

                // Sort the data based on the specified field.
                List<WifiLibrary>? sortedLibraries = SortData(wifiLibraries, _field);

                // Provide user feedback and next steps.
                await ProvideFeedback(botClient, message, cancellationToken, sortedLibraries);

                // Save sorted libraries for the user.
                _userStateManager.SetUserLibraries(message.From.Id, sortedLibraries ?? new List<WifiLibrary>());
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors.
                // Outputting a message to a file and to the console.
                logger.LogError($"{ex.Message}\nИспользуйте команду /start для перезапуска бота.");
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"{ex.Message}\nИспользуйте команду /start для перезапуска бота.", cancellationToken: cancellationToken);
            }

            _userStateManager.SetUserState(message.From.Id, UserState.ProcessingCompleted);
        }

        /// <summary>
        /// Sorts the data from the file based on the specified field.
        /// </summary>
        /// <param name="wifiLibraries">The list of WifiLibrary objects to sort.</param>
        /// <param name="field">The field to sort by.</param>
        /// <returns>A sorted list of WifiLibrary objects.</returns>
        private List<WifiLibrary>? SortData(List<WifiLibrary> wifiLibraries, string field)
        {
            // Sorting logic based on the field.
            switch (field)
            {
                case "LibraryName":
                    return wifiLibraries.OrderBy(lib => lib.LibraryName).ToList();
                case "CoverageArea":
                    return wifiLibraries.OrderByDescending(lib => lib.CoverageArea).ToList();
                default:
                    return null; 
            }
        }

        /// <summary>
        /// Provides feedback to the user based on the outcome of the sorting process.
        /// </summary>
        /// <param name="botClient">The Telegram bot client instance.</param>
        /// <param name="message">The message object.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <param name="sortedLibraries">The result of the sorting process.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ProvideFeedback(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken, List<WifiLibrary>? sortedLibraries)
        {
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
                {
                new KeyboardButton[] { "/download" },
                new KeyboardButton[] { "/help" },
                new KeyboardButton[] { "/start" },
                })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };

            if (sortedLibraries == null || !sortedLibraries.Any())
            {
                // Inform the user if no matching records were found.
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Результат сортировки пуст.", replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);
            }
            else
            {
                var responseText = string.Join("\n", sortedLibraries.Select(lib => lib.ToString()));

                _userStateManager.SetUserState(message.From.Id, UserState.ProcessingCompleted);

                // Inform the user that the sorting was successful.
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Сортировка произведена успешно!\nДля загрузки обработанного файла используйте команду /download", replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);
            }
        }
    }
}
