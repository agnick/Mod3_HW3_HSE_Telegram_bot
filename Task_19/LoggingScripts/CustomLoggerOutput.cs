using Microsoft.Extensions.Logging;

namespace Task_19.LoggingScripts
{
    public static class CustomLoggerOutput
    {
        /// <summary>
        /// Configures and returns an ILoggerFactory that includes both console and custom file logging.
        /// </summary>
        /// <param name="writer">The StreamWriter used for writing logs to a text file.</param>
        /// <returns>An ILoggerFactory configured to log to the console and a text file.</returns>
        /// <remarks>
        /// This method sets up logging to the console with specific formatting options and also
        /// adds a custom logging provider that writes logs to a file. This allows logs to be
        /// captured both in real time on the console and persistently in a text file for later review.
        /// </remarks>
        public static ILoggerFactory GetFactory(StreamWriter writer)
        {
            // Create an ILoggerFactory instance.
            return LoggerFactory.Create(builder =>
            {
                // Add console logging with custom settings.
                builder.AddSimpleConsole(options =>
                {
                    // Include scopes in the log output.
                    options.IncludeScopes = true;
                    // Format each log entry as a single line.
                    options.SingleLine = true;
                    // Set the format for timestamps in log messages.
                    options.TimestampFormat = "HH:mm:ss ";
                });

                // Add a custom logging provider that writes logs to text files, using the provided StreamWriter.
                builder.AddProvider(new CustomFileLoggerProvider(writer));
            });
        }
    }
}