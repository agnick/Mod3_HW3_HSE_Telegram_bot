using Microsoft.Extensions.Logging;

namespace Task_19.LoggingScripts
{
    // Customized ILogger, writes logs to text files
    public class CustomFileLogger : ILogger
    {
        // Represents the category name for messages produced by the logger.
        private readonly string _categoryName;
        // Handles writing log messages to a file.
        private readonly StreamWriter _logFileWriter;

        // Initializes a new instance of the CustomFileLogger class.
        public CustomFileLogger(string categoryName, StreamWriter logFileWriter)
        {
            _categoryName = categoryName;
            _logFileWriter = logFileWriter;
        }

        /// <summary>
        /// Begins a logical operation scope. This method is not used in this logger implementation.
        /// </summary>
        /// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
        /// <param name="state">The state for the started scope.</param>
        /// <returns>A disposable object representing the scope.</returns>
        public IDisposable? BeginScope<TState>(TState state)
        {
            return null;
        }

        /// <summary>
        /// Checks if the given log level is enabled.
        /// </summary>
        /// <param name="logLevel">The level to check.</param>
        /// <returns>True if logging is enabled for the specified level, false otherwise.</returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            // Ensure that only information level and higher logs are recorded.
            return logLevel >= LogLevel.Information;
        }

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <typeparam name="TState">The type of the object to be written.</typeparam>
        /// <param name="logLevel">The severity level of the log entry.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="state">The entry to be logged.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">Function to create a string message of the state and exception.</param>
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            // Ensure that only information level and higher logs are recorded.
            if (!IsEnabled(logLevel))
            {
                return;
            }

            // Get the formatted log message.
            var message = formatter(state, exception);

            //Write log messages to text file.
            _logFileWriter.WriteLine($"[{logLevel}] [{_categoryName}] {message}");
            _logFileWriter.Flush();
        }
    }
}