using DataProcessing;

namespace Task_19.TelegramBotScripts.Helpers
{
    // Represents the various states a user can be in during interaction with the Telegram bot.
    public enum UserState
    {
        // User has started interaction but has not performed any actions yet.
        Initial,
        // User has successfully uploaded a file.
        FileUploaded,
        // User is expected to upload a file.
        AwaitingFileUpload,
        // User needs to make a selection from a given set of options.
        AwaitingSelection,
        // User is awaiting to provide sorting preferences.
        AwaitingSorting,
        // User is expected to input a value for a specific field.
        AwaitingFieldValue,
        // User is ready to download processed data.
        AwaitingDownloading,
        // Data processing is completed, and the user can proceed with further actions.
        ProcessingCompleted
    }

    public class UserStateManager
    {
        // // Maps user ID to their current state.
        private readonly Dictionary<long, UserState> _userStates = new Dictionary<long, UserState>();
        // Stores files uploaded by users.
        private readonly Dictionary<long, Stream> _userFiles = new Dictionary<long, Stream>();
        // Stores selected fields by users for operations.
        private readonly Dictionary<long, string> _userFields = new Dictionary<long, string>();
        // Stores processed data for each user.
        private readonly Dictionary<long, List<WifiLibrary>> _userLibraries = new Dictionary<long, List<WifiLibrary>>();

        // Constructor to initialize an empty instance of the JSONProcessing class.
        public UserStateManager() { }

        /// <summary>
        /// Updates or sets the state of a specified user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="state">The new state to assign to the user.</param>
        public void SetUserState(long userId, UserState state)
        {
            _userStates[userId] = state;
        }

        /// <summary>
        /// Associates a file stream with a specified user. If a file already exists, it is disposed before updating.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="fileStream">The file stream to associate with the user.</param>
        public void SetUserFile(long userId, Stream fileStream)
        {
            if (_userFiles.ContainsKey(userId))
            {
                // Dispose the existing file stream if present.
                _userFiles[userId]?.Dispose(); 
            }

            _userFiles[userId] = fileStream;
        }

        /// <summary>
        /// Sets a specific field for a user to filter or sort data.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="field">The field selected by the user.</param>
        public void SetUserField(long userId, string field)
        {
            _userFields[userId] = field;
        }

        /// <summary>
        /// Stores processed libraries data for a specified user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="userLibraries">The list of WifiLibrary objects to store for the user.</param>
        public void SetUserLibraries(long userId, List<WifiLibrary> userLibraries)
        {
            _userLibraries[userId] = userLibraries;
        }

        /// <summary>
        /// Retrieves the file stream associated with a specified user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>The file stream if present; otherwise, null.</returns>
        public Stream? GetUserFile(long userId)
        {
            _userFiles.TryGetValue(userId, out Stream? fileStream);

            return fileStream;
        }

        /// <summary>
        /// Retrieves the field selected by the user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>The selected field if present; otherwise, null.</returns>
        public string? GetUserField(long userId)
        {
            _userFields.TryGetValue(userId, out string? field);

            return field;
        }

        /// <summary>
        /// Retrieves the current state of a specified user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>The current state of the user. Returns 'Initial' state if the user's state has not been set.</returns>
        public UserState GetUserState(long userId)
        {
            return _userStates.TryGetValue(userId, out var state) ? state : UserState.Initial;
        }

        /// <summary>
        /// Retrieves the list of WifiLibrary objects associated with a specified user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>The list of WifiLibrary objects if present; otherwise, null.</returns>
        public List<WifiLibrary>? GetUserLibraries(long userId)
        {
            _userLibraries.TryGetValue(userId, out List<WifiLibrary>? list);

            return list;
        }
    }
}