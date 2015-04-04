using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Samotorcan.HtmlUi.Core.Diagnostics;
using Samotorcan.HtmlUi.Core.Logs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Utilities
{
    /// <summary>
    /// JSON utility.
    /// </summary>
    internal static class JsonUtility
    {
        #region Properties
        #region Private

        #region Serializer
        /// <summary>
        /// Gets or sets the serializer.
        /// </summary>
        /// <value>
        /// The serializer.
        /// </value>
        private static JsonSerializer Serializer { get; set; }
        #endregion
        #region JsonSerializerSettings
        /// <summary>
        /// Gets or sets the json serializer settings.
        /// </summary>
        /// <value>
        /// The json serializer settings.
        /// </value>
        private static JsonSerializerSettings JsonSerializerSettings { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes the <see cref="JsonUtility"/> class.
        /// </summary>
        static JsonUtility()
        {
            JsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> { new KeyValuePairConverter() }
            };
            Serializer = JsonSerializer.Create(JsonSerializerSettings);
        }

        #endregion
        #region Methods
        #region Public

        #region SerializeToBson
        /// <summary>
        /// Serializes to BSON.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">value</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "It's not.")]
        public static byte[] SerializeToBson(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            using (var memoryStream = new MemoryStream())
            {
                using (BsonWriter writer = new BsonWriter(memoryStream))
                    Serializer.Serialize(writer, value);

                return memoryStream.ToArray();
            }
        }
        #endregion
        #region DeserializeFromBson
        /// <summary>
        /// Deserializes from BSON.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="readRootValueAsArray">if set to <c>true</c> read root value as array.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">value</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "It's not.")]
        public static TType DeserializeFromBson<TType>(byte[] value, bool readRootValueAsArray)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            using (var memoryStream = new MemoryStream(value))
            {
                using (BsonReader reader = new BsonReader(memoryStream, readRootValueAsArray, DateTimeKind.Utc))
                    return Serializer.Deserialize<TType>(reader);
            }
        }

        /// <summary>
        /// Deserializes from BSON.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static TType DeserializeFromBson<TType>(byte[] value)
        {
            return JsonUtility.DeserializeFromBson<TType>(value, false);
        }
        #endregion
        #region SerializeToJson
        /// <summary>
        /// Serializes to json.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string SerializeToJson(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented, JsonSerializerSettings);
        }
        #endregion
        #region SerializeToByteJson
        /// <summary>
        /// Serializes to byte json.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static byte[] SerializeToByteJson(object value)
        {
            return Encoding.UTF8.GetBytes(JsonUtility.SerializeToJson(value));
        }
        #endregion

        #endregion
        #endregion
    }
}
