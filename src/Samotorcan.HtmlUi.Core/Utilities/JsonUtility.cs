using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                JsonSerializer serializer = new JsonSerializer();

                using (BsonWriter writer = new BsonWriter(memoryStream))
                    serializer.Serialize(writer, value);

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
                JsonSerializer serializer = new JsonSerializer();

                using (BsonReader reader = new BsonReader(memoryStream, readRootValueAsArray, DateTimeKind.Utc))
                    return serializer.Deserialize<TType>(reader);
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
        public static byte[] SerializeToJson(object value)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
        }
        #endregion

        #endregion
        #endregion
    }
}
