using Task_19.TelegramBotScripts;
using Microsoft.Extensions.Logging;
using Task_19.LoggingScripts;

namespace Task_19
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Define the path to the text file for logging purposes.
            string logFilePath = Path.Combine("..", "..", "..", "..", "var", "console_log.txt");
            // The API token for the Telegram bot, obtained from BotFather.
            string apiToken = "7021824830:AAGTg6pdoxGQ5puHKkw93GDEkMa_4CdjNbM";

            try
            {
                // Create a StreamWriter to write logs to a text file, enabling appending to existing logs.
                using StreamWriter logFileWriter = new StreamWriter(logFilePath, append: true);

                // Create a logger factory to manage logging, using a custom implementation that writes to the provided StreamWriter.
                ILoggerFactory loggerFactory = CustomLoggerOutput.GetFactory(logFileWriter);

                // Instantiate the Telegram bot with the provided API token and logger factory, then start receiving messages.
                TelegramBotStarter telegramBot = new TelegramBotStarter(apiToken, loggerFactory);
                await telegramBot.StartReceiving();
            }
            catch (Exception ex) 
            {
                // If an exception occurs during bot setup or message receiving, write the exception message to the console.
                Console.WriteLine(ex.Message);
            }           
        }
    }
}