using Samotorcan.HtmlUi.Core.Exceptions;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// File and assembly content provider.
    /// </summary>
    public class FileAssemblyContentProvider : IContentProvider
    {
        #region Properties
        #region Public

        #region ContentSearch
        /// <summary>
        /// Gets or sets the content search.
        /// </summary>
        /// <value>
        /// The content search.
        /// </value>
        public ContentSearch ContentSearch { get; set; }
        #endregion

        #endregion
        #endregion
        #region Methods
        #region Public

        #region GetContent
        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "It's ok.")]
        public byte[] GetContent(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");

            if (!path.StartsWith("/"))
                throw new ArgumentException("Must be an absolute path.", "path");

            if (!PathUtility.IsFullFileName(path))
                throw new ArgumentException("Invalid path.", "path");

            // actual file
            if (ContentSearch == ContentSearch.FileAndAssembly || ContentSearch == ContentSearch.File)
            {
                var filePath = PathUtility.WorkingDirectory + path;

                if (File.Exists(filePath))
                    return File.ReadAllBytes(filePath);
            }

            // assembly file
            if (ContentSearch == ContentSearch.FileAndAssembly || ContentSearch == ContentSearch.Assembly)
            {
                var assembly = Assembly.GetEntryAssembly();
                var resourceName = assembly.GetName().Name + path.Replace('/', '.');

                if (assembly.GetManifestResourceNames().Contains(resourceName))
                {
                    using (var stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        using(var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);

                            return memoryStream.ToArray();
                        }
                    }
                }
            }

            // HtmlUi resource
            if (ResourceUtility.ResourceExists(path))
                return ResourceUtility.GetResourceAsBytes(path);

            throw new ContentNotFoundException(path);
        }
        #endregion
        #region ContentExists
        /// <summary>
        /// Content exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public bool ContentExists(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");

            if (!path.StartsWith("/"))
                throw new ArgumentException("Must be an absolute path.", "path");

            if (!PathUtility.IsFullFileName(path))
                throw new ArgumentException("Invalid path.", "path");

            // actual file
            if (ContentSearch == ContentSearch.FileAndAssembly || ContentSearch == ContentSearch.File)
            {
                var filePath = PathUtility.WorkingDirectory + path;

                if (File.Exists(filePath))
                    return true;
            }

            // assembly file
            if (ContentSearch == ContentSearch.FileAndAssembly || ContentSearch == ContentSearch.Assembly)
            {
                var assembly = Assembly.GetEntryAssembly();
                var resourceName = assembly.GetName().Name + path.Replace('/', '.');

                if (assembly.GetManifestResourceNames().Contains(resourceName))
                    return true;
            }

            // HtmlUi resource
            if (ResourceUtility.ResourceExists(path))
                return true;

            return false;
        }
        #endregion
        #region GetUrlFromContentPath
        /// <summary>
        /// Gets the URL from content path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public string GetUrlFromContentPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");

            if (!path.StartsWith("/"))
                throw new ArgumentException("Must be an absolute path.", "path");

            if (!PathUtility.IsFullFileName(path))
                throw new ArgumentException("Invalid path.", "path");

            return path;
        }
        #endregion
        #region GetContentPathFromUrl
        /// <summary>
        /// Gets the content path from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public string GetContentPathFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            if (!PathUtility.IsFullFileName(url))
                throw new ArgumentException("Invalid url.", "url");

            return "/" + url.TrimStart('/');
        }
        #endregion

        #endregion
        #endregion
    }
}
