using Samotorcan.HtmlUi.Core.Utilities;
using Samotorcan.HtmlUi.Core.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            Argument.NullOrEmpty(path, "path");
            Argument.InvalidArgument(!path.StartsWith("/"), "Must be an absolute path.", "path");

            Argument.InvalidArgument(!PathUtility.IsFullFileName(path), "Invalid path.", "path");

            // actual file
            if (ContentSearch == ContentSearch.FileAndAssembly || ContentSearch == ContentSearch.File)
            {
                var filePath = PathUtility.NormalizedWorkingDirectory + path;

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
            path = ReleasePathIfNeeded(path);

            if (ResourceUtility.ResourceExists(path))
                return ResourceUtility.GetResourceAsBytes(path);

            throw new ArgumentException("Content was not found.", "path");
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
            Argument.NullOrEmpty(path, "path");
            Argument.InvalidArgument(!path.StartsWith("/"), "Must be an absolute path.", "path");

            Argument.InvalidArgument(!PathUtility.IsFullFileName(path), "Invalid path.", "path");

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
            Argument.NullOrEmpty(url, "url");
            Argument.InvalidArgument(!PathUtility.IsFullFileName(url), "Invalid url.", "url");

            return "/" + url.TrimStart('/');
        }
        #endregion

        #endregion
        #region Private

        #region ReleasePathIfNeeded
        /// <summary>
        /// Release path if needed.
        /// </summary>
        /// <param name="localPath">The local path.</param>
        /// <returns></returns>
        private string ReleasePathIfNeeded(string localPath)
        {         
#if !DEBUG
            if (localPath.EndsWith(".js"))
            {
                var minLocalPath = localPath.Substring(0, localPath.Length - 2) + "min.js";

                if (ResourceUtility.ResourceExists(minLocalPath))
                    return minLocalPath;
            }
#endif
            return localPath;
        }
        #endregion

        #endregion
        #endregion
    }
}
