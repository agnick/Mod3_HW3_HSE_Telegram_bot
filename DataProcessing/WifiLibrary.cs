using System.Globalization;
using System.Text.Json.Serialization;

namespace DataProcessing
{
    public class WifiLibrary
    {
        /* Fields */
        // Using JsonPropertyName for correct deserializatian.
        [JsonPropertyName("ID")]
        private int _id;
        [JsonPropertyName("LibraryName")]
        private string _libraryName;
        [JsonPropertyName("AdmArea")]
        private string _admArea;
        [JsonPropertyName("District")]
        private string _district;
        [JsonPropertyName("Address")]
        private string _address;
        [JsonPropertyName("NumberOfAccessPoints")]
        private int _numberOfAccessPoints;
        [JsonPropertyName("WiFiName")]
        private string _wiFiName;
        [JsonPropertyName("CoverageArea")]
        private double _coverageArea;
        [JsonPropertyName("FunctionFlag")]
        private string _functionFlag;
        [JsonPropertyName("AccessFlag")]
        private string _accessFlag;
        [JsonPropertyName("Password")]
        private string _password;
        [JsonPropertyName("Latitude_WGS84")]
        private double _latitudeWGS84;
        [JsonPropertyName("Longitude_WGS84")]
        private double _longitudeWGS84;
        [JsonPropertyName("global_id")]
        private double _globalId;
        [JsonPropertyName("geodata_center")]
        private string _geodataCenter;
        [JsonPropertyName("geoarea")]
        private string _geoArea;

        /* Properties */
        public int ID => _id;
        public string LibraryName => _libraryName;
        public string AdmArea => _admArea;
        public string District => _district;
        public string Address => _address;
        public int NumberOfAccessPoints => _numberOfAccessPoints;
        public string WiFiName => _wiFiName;
        public double CoverageArea => _coverageArea;
        public string FunctionFlag => _functionFlag;
        public string AccessFlag => _accessFlag;
        public string Password => _password;
        public double LatitudeWGS84 => _latitudeWGS84;
        public double LongitudeWGS84 => _longitudeWGS84;
        public double GlobalId => _globalId;
        public string GeoDataCenter => _geodataCenter;
        public string GeoArea => _geoArea;

        // Constructor to initialize an instance of the WifiLibrary class with provided values.
        // Using JsonConstructor for correct deserializatian.
        [JsonConstructor]
        public WifiLibrary(
            int id,
            string libraryName,
            string admArea,
            string district,
            string address,
            int numberOfAccessPoints,
            string wiFiName,
            double coverageArea,
            string functionFlag,
            string accessFlag,
            string password,
            double latitudeWGS84,
            double longitudeWGS84,
            double globalId,
            string geodataCenter,
            string geoArea)
        {
            // Constructor with parameters, validating and assigning values.
            if (id < 0) throw new ArgumentOutOfRangeException("Поле ID не может быть меньше нуля.");
            if (libraryName is null) throw new ArgumentNullException("Поле LibraryName не может быть null.");
            if (admArea is null) throw new ArgumentNullException("Поле AdmArea не может быть null.");
            if (district is null) throw new ArgumentNullException("Поле District не может быть null.");
            if (address is null) throw new ArgumentNullException("Поле Address не может быть null.");
            if (numberOfAccessPoints < 0) throw new ArgumentOutOfRangeException("Поле NumberOfAccessPoints не может быть меньше нуля.");
            if (wiFiName is null) throw new ArgumentNullException("Поле WiFiName не может быть null.");
            if (coverageArea < 0) throw new ArgumentOutOfRangeException("Поле CoverageArea не может быть меньше нуля.");
            if (functionFlag is null) throw new ArgumentNullException("Поле FunctionFlag не может быть null.");
            if (accessFlag is null) throw new ArgumentNullException("Поле AccessFlag не может быть null.");
            if (password is null) throw new ArgumentNullException("Поле Password не может быть null.");
            if (latitudeWGS84 < -90 || latitudeWGS84 > 90) throw new ArgumentOutOfRangeException(nameof(latitudeWGS84), "Поле Latitude_WGS84 должно быть в диапазоне от -90 до 90.");
            if (longitudeWGS84 < -180 || longitudeWGS84 > 180) throw new ArgumentOutOfRangeException(nameof(longitudeWGS84), "Поле Longitude_WGS84 должно быть в диапазоне от -180 до 180.");
            if (globalId < 0) throw new ArgumentOutOfRangeException(nameof(globalId), "Поле global_id не может быть меньше нуля.");
            if (geodataCenter is null) throw new ArgumentNullException("Поле geodata_center не может быть null.");
            if (geoArea is null) throw new ArgumentNullException("Поле geoarea не может быть null.");

            _id = id;
            _libraryName = libraryName;
            _admArea = admArea;
            _district = district;
            _address = address;
            _numberOfAccessPoints = numberOfAccessPoints;
            _wiFiName = wiFiName;
            _coverageArea = coverageArea;
            _functionFlag = functionFlag;
            _accessFlag = accessFlag;
            _password = password;
            _latitudeWGS84 = latitudeWGS84;
            _longitudeWGS84 = longitudeWGS84;
            _globalId = globalId;
            _geodataCenter = geodataCenter;
            _geoArea = geoArea;
        }

        // Constructor to initialize an empty instance of the WifiLibrary class.
        public WifiLibrary()
        {
            _id = 0;
            _libraryName = string.Empty;
            _admArea = string.Empty;
            _district = string.Empty;
            _address = string.Empty;
            _numberOfAccessPoints = 0;
            _wiFiName = string.Empty;
            _coverageArea = 0;
            _functionFlag = string.Empty;
            _accessFlag = string.Empty;
            _password = string.Empty;
            _latitudeWGS84 = 0;
            _longitudeWGS84 = 0;
            _globalId = 0;
            _geodataCenter = string.Empty;
            _geoArea = string.Empty;
        }

        /// <summary>
        /// Converts the WifiLibrary object to a CSV-formatted string.
        /// </summary>
        /// <returns>A CSV-formatted string representing the WifiLibrary object.</returns>
        public override string ToString()
        {
            return $"\"{_id}\";\"{_libraryName}\";\"{_admArea}\";\"{_district}\";\"{_address}\";\"{_numberOfAccessPoints}\";\"{_wiFiName}\";\"{_coverageArea}\";\"{_functionFlag}\";\"{_accessFlag}\";\"{_password}\";\"{_latitudeWGS84.ToString(CultureInfo.InvariantCulture)}\";\"{_longitudeWGS84.ToString(CultureInfo.InvariantCulture)}\";\"{_globalId}\";\"{_geodataCenter}\";\"{_geoArea}\";";
        }
    }
}