using System.Globalization;
using System.Text;

namespace DataProcessing
{
    public class CSVProcessing
    {
        // Сonstants that store the first and second lines of the file.
        const string firstLine = "\"ID\";\"LibraryName\";\"AdmArea\";\"District\";\"Address\";\"NumberOfAccessPoints\";\"WiFiName\";\"CoverageArea\";\"FunctionFlag\";\"AccessFlag\";\"Password\";\"Latitude_WGS84\";\"Longitude_WGS84\";\"global_id\";\"geodata_center\";\"geoarea\";";
        const string secondLine = "\"Код\";\"Наименование библиотеки\";\"Административный округ\";\"Район\";\"Адрес\";\"Количество точек доступа\";\"Имя Wi-Fi сети\";\"Зона покрытия, в метрах\";\"Признак функционирования\";\"Условия доступа\";\"Пароль\";\"Широта в WGS-84\";\"Долгота в WGS-84\";\"global_id\";\"geodata_center\";\"geoarea\";";

        // Constructor to initialize an empty instance of the JSONProcessing class.
        public CSVProcessing() { }

        /// <summary>
        /// Serializes a list of WifiLibrary objects to a CSV format string and writes it to a memory stream.
        /// </summary>
        /// <param name="libraries">A list of WifiLibrary objects to be serialized to CSV format.</param>
        /// <returns>A memory stream containing the serialized CSV string of the list of WifiLibrary objects.</returns>
        /// <exception cref="Exception">Throws an exception if an error occurs during the serialization or stream writing process.</exception>
        public Stream Write(List<WifiLibrary> libraries)
        {
            try
            {
                StringBuilder csvContent = new StringBuilder();
                // Assuming 'firstLine' contains column headers or similar initial CSV line.
                csvContent.AppendLine(firstLine);
                // Assuming 'secondLine' contains additional headers or initial setup.
                csvContent.AppendLine(secondLine); 

                foreach (var library in libraries)
                {
                    // Append each WifiLibrary object's string representation, assuming it follows CSV format.
                    csvContent.AppendLine(library.ToString()); 
                }

                // Convert string builder content to memory stream.
                Stream? stream = new MemoryStream();
                // Use StreamWriter with UTF8 encoding to write to the stream.
                StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(true));

                // Write the CSV content to the stream.
                writer.Write(csvContent.ToString());
                // Ensure all buffered data is written to the stream.
                writer.Flush();
                // Reset the stream position to the beginning for reading.
                stream.Position = 0; 

                return stream; 
            }
            catch (Exception ex)
            {
                throw new Exception("Произошла ошибка при формировании потока записи"); 
            }
        }

        /// <summary>
        /// Reads a CSV format stream and deserializes it into a list of WifiLibrary objects.
        /// </summary>
        /// <param name="stream">The stream to read the CSV data from.</param>
        /// <returns>A list of WifiLibrary objects deserialized from the CSV data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the input stream is null or if an error occurs during reading, indicating the specific cause of the failure.</exception>
        public List<WifiLibrary> Read(Stream stream)
        {
            try
            {
                // Initialize a list to hold lines of data read from the stream.
                List<string> rowData = new List<string>(); 

                using (StreamReader reader = new StreamReader(stream))
                {
                    string line;
                    // Read each line from the stream until no more lines are found.
                    while ((line = reader.ReadLine()) != null) 
                    {
                        // Add each line to the rowData list.
                        rowData.Add(line); 
                    }
                }

                // Verify that the file structure is as expected.
                CheckFileStructure(rowData);
                // Deserialize the CSV data into a list of WifiLibrary objects.
                List<WifiLibrary> wifiLibraries = DeserializeCsv(rowData);

                return wifiLibraries;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Возникла ошибка при десереализации данных из файла.");
            }
        }

        /// <summary>
        /// A method that checks the structure of the transmitted file.
        /// </summary>
        /// <param name="rowData">An array of strings read from a file.</param>
        /// <exception cref="ArgumentNullException">An exception that is thrown when the file structure is violated.</exception>
        private void CheckFileStructure(List<string> rowData)
        {
            // Checking that the transferred file is not empty.
            if (rowData.Count == 0)
            {
                throw new ArgumentException("Передан пустой файл.");
            }

            // Checking that the first two lines of the transferred file correspond to the first two lines of the variant file.
            if (rowData[0] != firstLine || rowData[1] != secondLine)
            {
                Console.WriteLine(rowData[0]);
                Console.WriteLine(rowData[1]);
                throw new ArgumentException("Поля переданного csv файла не соответствуют полям варианта.");
            }

            // Checking each subsequent line of the transferred file for compliance with the variant file.
            foreach (string line in rowData.GetRange(2, rowData.Count - 2))
            {
                // Forming an array of elements from a file line separated by a semicolon.
                string[] splitted = line.Split(";", StringSplitOptions.RemoveEmptyEntries);

                // Checking that the number of elements in the line corresponds to the number of elements in the line of the variant file.
                if (splitted.Length != 16)
                {
                    throw new ArgumentException("Данные в файле не соответствуют варианту.");
                }

                // Checking the data type of each line element against the variant file.
                for (int i = 0; i < splitted.Length; i++)
                {
                    // Removing quotes from a string.
                    string dataElement = splitted[i].Replace("\"", "");
                    // An array storing field numbers, elements that have numeric values.
                    int[] toCheck = { 0, 5, 7, 11, 12, 13 };
                    // Сhecking only those elements that have numerical values ​​in the variant file, like the rest of the lines.
                    if (toCheck.Contains(i))
                    {
                        if (dataElement != "" && !double.TryParse(dataElement, NumberStyles.Float, CultureInfo.InvariantCulture, out double _))
                        {
                            throw new ArgumentException("Данные в файле не соответствуют варианту.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deserializes a list of CSV-formatted string lines into a list of WifiLibrary objects.
        /// </summary>
        /// <param name="rowData">A list of string lines, each representing a row of CSV data.</param>
        /// <returns>A list of WifiLibrary objects created from the CSV data.</returns>
        /// <remarks>
        /// This method assumes that the CSV data starts from the third line, skipping the first two lines which might be headers or other metadata.
        /// Each line is split into elements using a semicolon (;) as the delimiter. It also handles the removal of quotation marks from the CSV fields
        /// and parses numeric values appropriately, taking into account cultural differences in number formatting.
        /// </remarks>
        private List<WifiLibrary> DeserializeCsv(List<string> rowData)
        {
            List<WifiLibrary> wifiLibraries = new List<WifiLibrary>();

            // Start from the third line, assuming the first two lines are headers or metadata.
            foreach (string line in rowData.GetRange(2, rowData.Count - 2))
            {
                // Split the line into individual elements, removing empty entries.
                string[] els = line.Split(";", StringSplitOptions.RemoveEmptyEntries);

                // Create a new WifiLibrary object from the CSV elements, converting strings to their appropriate data types.
                wifiLibraries.Add(new WifiLibrary(
                    int.Parse(els[0].Replace("\"", "")),
                    els[1].Replace("\"", ""),
                    els[2].Replace("\"", ""),
                    els[3].Replace("\"", ""),
                    els[4].Replace("\"", ""),
                    int.Parse(els[5].Replace("\"", "")),
                    els[6].Replace("\"", ""),
                    double.Parse(els[7].Replace("\"", ""), NumberStyles.Float, CultureInfo.InvariantCulture),
                    els[8].Replace("\"", ""),
                    els[9].Replace("\"", ""),
                    els[10].Replace("\"", ""),
                    double.Parse(els[11].Replace("\"", ""), NumberStyles.Float, CultureInfo.InvariantCulture),
                    double.Parse(els[12].Replace("\"", ""), NumberStyles.Float, CultureInfo.InvariantCulture),
                    double.Parse(els[13].Replace("\"", ""), NumberStyles.Float, CultureInfo.InvariantCulture),
                    els[14].Replace("\"", ""),
                    els[15].Replace("\"", "")
                ));
            }

            return wifiLibraries;
        }
    }
}