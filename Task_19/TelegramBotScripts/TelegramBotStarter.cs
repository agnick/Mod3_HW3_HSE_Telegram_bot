using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Task_19.TelegramBotScripts.Handlers;
using Task_19.TelegramBotScripts.Interfaces;
using Task_19.TelegramBotScripts.Helpers;
using Microsoft.Extensions.Logging;

namespace Task_19.TelegramBotScripts
{
    public class TelegramBotStarter
    {
        /* Fields */
        // Represents the client that connects to the Telegram Bot API.
        private readonly TelegramBotClient _botClient;
        // Maps command texts to their corresponding handlers.
        private readonly Dictionary<string, ICommandHandler> _commandHandlers = new Dictionary<string, ICommandHandler>();
        // Manages state of conversation with each user.
        private readonly UserStateManager _userStateManager = new UserStateManager();
        // Helps in determining and handling file types.
        private readonly FileTypeManager _fileTypeManager = new FileTypeManager();
        // Allows cancellation of asynchronous operations.
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        // Allows logging of messages and errors to a file and to the console.
        private readonly ILoggerFactory _loggerFactory;

        // Constructor to initialize an instance of the TelegramBotStarter class with provided values.
        public TelegramBotStarter(string token, ILoggerFactory loggerFactory)
        {
            // Initialize the bot client with the provided token.
            _botClient = new TelegramBotClient(token);
            // Initialize logger factory.
            _loggerFactory = loggerFactory;
            // Setup the command handlers.
            InitializeCommandHandlers();
        }

        /// <summary>
        /// Starts the bot's message receiving process and keeps the application running indefinitely to listen for updates.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation of message reception.</returns>
        public async Task StartReceiving()
        {
            // Configures the bot to receive all update types except ChatMember related updates.
            ReceiverOptions receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() 
            };

            // Start receiving updates using the configured options and handling methods.
            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: _cts.Token
            );

            // Get the bot's user info.
            var me = await _botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");

            // Wait indefinitely, keeping the application running.
            await Task.Delay(Timeout.Infinite, _cts.Token);
        }

        /// <summary>
        /// Initializes command handlers for different bot commands by mapping them to their specific implementations.
        /// </summary>
        private void InitializeCommandHandlers()
        {
            /*  
                Map commands to their respective handlers.
                Each command string key maps to a specific ICommandHandler implementation. 
                // This allows for modular handling of different bot commands based on the user's current state and input.
            */
            _commandHandlers["/uploadFile"] = new FileUploadHandler(_userStateManager);
            _commandHandlers["/select"] = new SelectionPreparingHandler(_userStateManager);
            _commandHandlers["/sort"] = new SortingPreparingHandler(_userStateManager);
            _commandHandlers["/download"] = new DownloadingPreparingHandler(_userStateManager);
            _commandHandlers["/AdmArea"] = new SelectionPickedHandler(_userStateManager, "AdmArea");
            _commandHandlers["/WiFiName"] = new SelectionPickedHandler(_userStateManager, "WiFiName");
            _commandHandlers["/FunctionFlagAndAccessFlag"] = new SelectionPickedHandler(_userStateManager, "FunctionFlagAndAccessFlag");
            _commandHandlers["/LibraryName"] = new DataSortingHandler(_userStateManager, _fileTypeManager, _loggerFactory, "LibraryName");
            _commandHandlers["/CoverageArea"] = new DataSortingHandler(_userStateManager, _fileTypeManager, _loggerFactory, "CoverageArea");
            _commandHandlers["/CSV"] = new FileDownloadHandler(_userStateManager, _loggerFactory, "CSV");
            _commandHandlers["/JSON"] = new FileDownloadHandler(_userStateManager, _loggerFactory, "JSON");
            _commandHandlers["/start"] = new StartCommandHandler(_userStateManager);
            _commandHandlers["/help"] = new HelpCommandHandler();
        }

        /// <summary>
        /// Sends a generic unsupported command message to the user.
        /// </summary>
        /// <param name="chatId">The chat ID where the message should be sent.</param>
        /// <returns>A task representing the asynchronous operation of sending the message.</returns>
        private async Task SendUnsupportedMessage(long chatId, string message)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Команда не поддерживается или не доступна сейчас. Введите /help, чтобы узнать список доступных команд."
            );

            // Logger initialization.
            ILogger logger = _loggerFactory.CreateLogger("TgBot");
            // Outputting a message to a file and to the console.
            logger.LogError($"Команда \"{message}\" не поддерживается или не доступна сейчас. Введите /help, чтобы узнать список доступных команд.");
        }

        /// <summary>
        /// Determines if a given command is available for the user's current state.
        /// </summary>
        /// <param name="command">The command to check.</param>
        /// <param name="userState">The current state of the user.</param>
        /// <returns>True if the command is available in the current state; otherwise, false.</returns>
        private bool CommandAvailableForState(string command, UserState userState)
        {
            // Logic to determine if the command can be executed in the current user state.
            // Helps in controlling the flow of the bot based on the state of the conversation with a user.
            switch (userState)
            {
                case UserState.Initial:
                    return command == "/uploadFile" || command == "/start" || command == "/help";
                case UserState.FileUploaded:
                    return command == "/select" || command == "/sort" || command == "/help" || command == "/start";
                case UserState.AwaitingSelection:
                    return command == "/AdmArea" || command == "/WiFiName" || command == "/FunctionFlagAndAccessFlag" || command == "/help" || command == "/start";
                case UserState.AwaitingSorting:
                    return command == "/LibraryName" || command == "/CoverageArea" || command == "/help" || command == "/start";
                case UserState.ProcessingCompleted:
                    return command == "/download" || command == "/help" || command == "/start";
                case UserState.AwaitingDownloading:
                    return command == "/CSV" || command == "/JSON" || command == "/help" || command == "/start";
                default:
                    return command == "/help" || command == "/start";
            }
        }

        /// <summary>
        /// Handles incoming updates from Telegram.
        /// </summary>
        /// <param name="botClient">The bot client instance.</param>
        /// <param name="update">The update to handle.</param>
        /// <param name="cancellationToken">A token for canceling the operation.</param>
        /// <returns>A task representing the asynchronous operation of update handling.</returns>
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;

            // Checks for further work.
            if (message == null || message.From == null) return;

            var userId = message.From.Id;

            // Logger initialization.
            ILogger logger = _loggerFactory.CreateLogger("TgBot");

            // Handling the case when the message type is a document is available only when the bot is expecting a document from the user (after the /upload command).
            if (message.Type == MessageType.Document && _userStateManager.GetUserState(message.From.Id) == UserState.AwaitingFileUpload)
            {
                if (message.Document == null) return;

                // Outputting a message to a file and to the console.
                logger.LogInformation($"Получен документ {message.Document.FileName} от пользователя {userId}");
                // Processing the sent file.
                await new FileReceivedHandler(_userStateManager, _fileTypeManager, _loggerFactory).HandleFileAsync(botClient, message, cancellationToken);
                return;
            }

            // Checks for further work.
            if (message.Type != MessageType.Text || message.Text == null)
            {
                // Outputting a message to a file and to the console.
                logger.LogWarning($"Тип данного сообщения от пользователя {userId} не может быть обработан");
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Тип данного сообщения не может быть обработан. Если вы отправляете файл, то сначала используйте команду /uploadFile", cancellationToken: cancellationToken);
                return;
            }

            // Processing the message.
            string[] commandText = message.Text.Split(';');
            UserState userState = _userStateManager.GetUserState(message.From.Id);
            var handler = _commandHandlers.GetValueOrDefault(commandText[0]);

            // Outputting a message to a file and to the console.
            logger.LogInformation($"Сообщение от пользователя {userId}: \"{message.Text}\"");

            // Handling a special case where a field is expected from the user.
            if (userState == UserState.AwaitingFieldValue)
            {
                // We get the user-selected field for filtering.
                var field = _userStateManager.GetUserField(userId);

                // Check for further work.
                if (field == null) return;

                // Organizing data selection based on the entered field.
                var dataHandler = new DataSelectionHandler(_userStateManager, _fileTypeManager, _loggerFactory, field, commandText); 
                await dataHandler.HandleCommandAsync(botClient, message, cancellationToken);

                return;
            }

            // If the command is available at this stage, then the handler of the called command is called, otherwise we display a hint message to the user.
            if (handler != null && CommandAvailableForState(commandText[0], userState))
            {
                await handler.HandleCommandAsync(botClient, message, cancellationToken);
            }
            else
            {
                // Display custom errors depending on available commands.
                if (userState == UserState.FileUploaded && commandText[0] == "/uploadFile")
                {
                    // Outputting a message to a file and to the console.
                    logger.LogError("Вы уже загрузили файл.");
                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Вы уже загрузили файл.", cancellationToken: cancellationToken);
                }
                else if ((userState == UserState.Initial || userState == UserState.AwaitingFileUpload) && (commandText[0] == "/select" || commandText[0] == "/sort" || commandText[0] == "/download"))
                {
                    // Outputting a message to a file and to the console.
                    logger.LogError("Сначала загрузите файл, соответствующий варианту.");
                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Сначала загрузите файл, соответствующий варианту.", cancellationToken: cancellationToken);
                }
                else
                {
                    await SendUnsupportedMessage(message.Chat.Id, message.Text);
                }
            }
        }

        /// <summary>
        /// Handles polling errors that occur while the bot is receiving updates.
        /// </summary>
        /// <param name="botClient">The bot client instance.</param>
        /// <param name="exception">The exception that occurred during polling.</param>
        /// <param name="cancellationToken">A token for canceling the operation.</param>
        /// <returns>A task representing the completion of error handling.</returns>
        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Logs errors encountered during the polling of updates and handles them appropriately.
            // Ensures that the bot remains operational even in the face of errors.
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            // Logger initialization.
            ILogger logger = _loggerFactory.CreateLogger("TgBot");
            // Outputting a message to a file and to the console.
            logger.LogError(errorMessage);

            return Task.CompletedTask;
        }
    }
}
