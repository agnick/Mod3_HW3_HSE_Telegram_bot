using Microsoft.Extensions.Logging;

namespace Task_19.LoggingScripts
{
    // Customized ILoggerProvider, writes logs to text files
    public class CustomFileLoggerProvider : ILoggerProvider
    {
        // Holds the StreamWriter used to write log messages to a file.
        private readonly StreamWriter _logFileWriter;

        // Initializes a new instance of the CustomFileLoggerProvider class.
        public CustomFileLoggerProvider(StreamWriter logFileWriter)
        {
            // Ensure the StreamWriter is not null to avoid errors during logging.
            _logFileWriter = logFileWriter ?? throw new ArgumentNullException(nameof(logFileWriter));
        }

        /// <summary>
        /// Creates a logger instance that writes to a text file.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>A CustomFileLogger instance configured to use the provided StreamWriter.</returns>
        /// <remarks>This method allows the logging framework to provide loggers to different parts of the application.</remarks>
        public ILogger CreateLogger(string categoryName)
        {
            // Return a new logger instance using the current StreamWriter and category name.
            return new CustomFileLogger(categoryName, _logFileWriter);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>Ensures that the StreamWriter is properly disposed when the logger provider is disposed.</remarks>
        public void Dispose()
        {
            // Ensure the StreamWriter is properly disposed to release resources.
            _logFileWriter.Dispose();
        }
    }
}