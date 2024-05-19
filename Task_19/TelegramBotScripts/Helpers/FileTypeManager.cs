namespace Task_19.TelegramBotScripts.Helpers
{
    // Defines file types supported by the Telegram bot for data processing and exporting.
    public enum FileType
    {
        // Represents Comma-Separated Values file format.
        CSV,
        // Represents JavaScript Object Notation file format.
        JSON
    }

    public class FileTypeManager
    {
        private readonly Dictionary<long, FileType> _userFileTypes = new Dictionary<long, FileType>();

        // Constructor to initialize an empty instance of the JSONProcessing class.
        public FileTypeManager() { }

        /// <summary>
        /// Retrieves the preferred file type for a given user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The preferred FileType of the user if set; otherwise, defaults to CSV.</returns>
        public FileType GetFileType(long userId)
        {
            // Tries to get the user's preferred file type, defaults to CSV if not set.
            return _userFileTypes.TryGetValue(userId, out FileType fileType) ? fileType : FileType.CSV;
        }

        /// <summary>
        /// Sets or updates the preferred file type for a given user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="fileType">The FileType preference to set for the user.</param>
        public void SetFileType(long userId, FileType fileType)
        {
            // Updates the dictionary with the user's preferred file type.
            _userFileTypes[userId] = fileType;
        }
    }
}