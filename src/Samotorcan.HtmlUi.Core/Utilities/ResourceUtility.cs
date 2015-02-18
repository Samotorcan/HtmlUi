using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core.Utilities
{
    /// <summary>
    /// Resource utility.
    /// </summary>
    internal static class ResourceUtility
    {
        #region Methods
        #region Public

        #region GetResourceAsString
        /// <summary>
        /// Gets the resource as string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "It's ok.")]
        public static string GetResourceAsString(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            name = "Samotorcan.HtmlUi.Core.Resources." + name;
            var assembly = typeof(ResourceUtility).Assembly;

            using (var stream = assembly.GetManifestResourceStream(name))
            {
                if (stream == null)
                    throw new ArgumentException("Resource not found.", "name");

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        #endregion
        #region GetResourceAsBytes
        /// <summary>
        /// Gets the resource as bytes.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public static byte[] GetResourceAsBytes(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            name = "Samotorcan.HtmlUi.Core.Resources." + name;
            var assembly = typeof(ResourceUtility).Assembly;

            using (var stream = assembly.GetManifestResourceStream(name))
            {
                if (stream == null)
                    throw new ArgumentException("Resource not found.", "name");

                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
        #endregion

        #endregion
        #endregion
    }
}
