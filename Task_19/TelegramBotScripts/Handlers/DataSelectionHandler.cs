using Task_19.TelegramBotScripts.Helpers;
using Task_19.TelegramBotScripts.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot;
using DataProcessing;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.Logging;

namespace Task_19.TelegramBotScripts.Handlers
{
    public class DataSelectionHandler : ICommandHandler
    {
        // Manages state of conversation with each user.
        private readonly UserStateManager _userStateManager;
        // Manages file type settings with each user.
        private readonly FileTypeManager _fileTypeManager;
        // Allows logging of messages and errors to a file and to the console.
        private readonly ILoggerFactory _loggerFactory;
        // Field to select by.
        private readonly string _field;
        // Field value(s) to select by.
        private readonly string[] _val;

        // Constructor to initialize an instance of the DataSortingHandler class with provided values.
        public DataSelectionHandler(UserStateManager userStateManager, FileTypeManager fileTypeManager, ILoggerFactory loggerFactory, string field, string[] val)
        {
            _userStateManager = userStateManager;
            _fileTypeManager = fileTypeManager;
            _loggerFactory = loggerFactory;
            _field = field;
            _val = val;
        }

        /// <summary>
        /// Asynchronously processes a selection command by filtering data in the uploaded file based on the specified criteria.
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

            // Ensure the command is from an actual user with an uploaded file.
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

                // Filter the data based on the specified field and values.
                List<WifiLibrary>? selectedLibraries = FilterData(wifiLibraries, _field, _val);

                // Provide user feedback and next steps.
                await ProvideFeedback(botClient, message, cancellationToken, selectedLibraries);

                // Save selected libraries for the user.
                _userStateManager.SetUserLibraries(message.From.Id, selectedLibraries ?? new List<WifiLibrary>());
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
        /// Filters the data based on the specified criteria.
        /// </summary>
        /// <param name="wifiLibraries">The list of WifiLibrary objects to filter.</param>
        /// <param name="field">The field to filter by.</param>
        /// <param name="val">The value(s) to use for filtering.</param>
        /// <returns>A filtered list of WifiLibrary objects.</returns>
        private List<WifiLibrary>? FilterData(List<WifiLibrary> wifiLibraries, string field, string[] val)
        {
            // Filtering logic based on the field and values.
            switch (field)
            {
                case "AdmArea":
                    return val.Length == 1 ? wifiLibraries.Where(lib => lib.AdmArea.Equals(val[0], StringComparison.OrdinalIgnoreCase)).ToList() : null;
                case "WiFiName":
                    return val.Length == 1 ? wifiLibraries.Where(lib => lib.WiFiName.Equals(val[0], StringComparison.OrdinalIgnoreCase)).ToList() : null;
                case "FunctionFlagAndAccessFlag":
                    return val.Length == 2 ? wifiLibraries.Where(lib => lib.FunctionFlag.Equals(val[0], StringComparison.OrdinalIgnoreCase) && lib.AccessFlag.Equals(val[1], StringComparison.OrdinalIgnoreCase)).ToList() : null;
                default:
                    return null; 
            }
        }

        /// <summary>
        /// Provides feedback to the user based on the outcome of the selection process.
        /// </summary>
        /// <param name="botClient">The Telegram bot client instance.</param>
        /// <param name="message">The message object.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <param name="selectedLibraries">The result of the selection process.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ProvideFeedback(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken, List<WifiLibrary>? selectedLibraries)
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

            if (selectedLibraries == null || !selectedLibraries.Any())
            {
                // Inform the user if no matching records were found.
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Результат выборки пуст.", replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);
            }
            else
            {
                var responseText = string.Join("\n", selectedLibraries.Select(lib => lib.ToString()));

                _userStateManager.SetUserState(message.From.Id, UserState.ProcessingCompleted);

                // Inform the user that the selection was successful.
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Выборка произведена успешно!\nДля загрузки обработанного файла используйте команду /download", replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);
            }
        }
    }
}