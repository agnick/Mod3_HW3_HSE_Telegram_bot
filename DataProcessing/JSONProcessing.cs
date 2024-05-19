using System.Text.Json;

namespace DataProcessing
{
    public class JSONProcessing
    {
        // Constructor to initialize an empty instance of the JSONProcessing class.
        public JSONProcessing() { }

        /// <summary>
        /// Serializes a list of WifiLibrary objects to a JSON string and writes it to a memory stream.
        /// </summary>
        /// <param name="libraries">A list of WifiLibrary objects to be serialized and written to a stream.</param>
        /// <returns>A memory stream containing the serialized JSON string of the list of WifiLibrary objects.</returns>
        /// <exception cref="Exception">Throws an exception if an error occurs during the serialization or writing process.</exception>
        public Stream Write(List<WifiLibrary> libraries)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    // Enable pretty-printing of JSON content.
                    WriteIndented = true,
                    // Use an encoder that allows a broader set of characters without escaping.
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping 
                };
                // Serialize the libraries list to a UTF-8 encoded byte array.
                byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(libraries, options);

                // Create a new memory stream.
                var stream = new MemoryStream();
                // Write the serialized byte array to the memory stream.
                stream.Write(jsonUtf8Bytes, 0, jsonUtf8Bytes.Length);

                // Reset the stream's position to the beginning to allow reading from start.
                stream.Position = 0; 

                return stream; 
            }
            catch (Exception ex)
            {
                throw new Exception("Произошла ошибка при формировании потока записи.");
            }
        }

        /// <summary>
        /// Deserializes the JSON data from a given stream into a list of WifiLibrary objects.
        /// </summary>
        /// <param name="stream">The stream containing the serialized JSON data of WifiLibrary objects.</param>
        /// <returns>A list of WifiLibrary objects deserialized from the JSON data in the stream. Returns null if deserialization fails.</returns>
        /// <exception cref="Exception">Throws an exception if an error occurs during the deserialization process.</exception>
        public List<WifiLibrary>? Read(Stream stream)
        {
            try
            {
                if (stream.CanSeek)
                {
                    // Reset the stream's position to the beginning to ensure it reads from the start.
                    stream.Seek(0, SeekOrigin.Begin); 
                }

                // Deserialize the JSON data from the stream into a list of WifiLibrary objects.
                List<WifiLibrary>? wifiLibraries = JsonSerializer.Deserialize<List<WifiLibrary>>(stream); 

                return wifiLibraries; 
            }
            catch (Exception ex)
            {
                throw new Exception("Произошла ошибка при десереализации данных из файла"); 
            }
        }
    }
}