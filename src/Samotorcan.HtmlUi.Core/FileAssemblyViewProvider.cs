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
    /// File and assembly view provider.
    /// </summary>
    public class FileAssemblyViewProvider : IViewProvider
    {
        #region Properties
        #region Public

        #region ViewSearch
        /// <summary>
        /// Gets or sets the view search.
        /// </summary>
        /// <value>
        /// The view search.
        /// </value>
        public ViewSearch ViewSearch { get; set; }
        #endregion

        #endregion
        #endregion
        #region Methods
        #region Public

        #region GetView
        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <param name="path">Name of the view.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">viewName</exception>
        /// <exception cref="System.ArgumentException">
        /// View name must start with ~/.;viewName
        /// or
        /// Invalid view name.;viewName
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "It's ok.")]
        public string GetView(string path)
        {
            Argument.NullOrEmpty(path, "path");
            Argument.InvalidArgument(!path.StartsWith("~/"), "View path must start with ~.", "path");

            var localViewPath = path.Substring(1);

            Argument.InvalidArgument(!PathUtility.IsFullFileName(localViewPath), "Invalid view path.", "path");

            // actual file
            if (ViewSearch == ViewSearch.FileAndAssembly || ViewSearch == ViewSearch.File)
            {
                var filePath = PathUtility.NormalizedWorkingDirectory + localViewPath;

                if (File.Exists(filePath))
                    return File.ReadAllText(filePath);
            }

            // assembly file
            if (ViewSearch == ViewSearch.FileAndAssembly || ViewSearch == ViewSearch.Assembly)
            {
                var assembly = Assembly.GetEntryAssembly();
                var resourceName = assembly.GetName().Name + localViewPath.Replace('/', '.');

                if (assembly.GetManifestResourceNames().Contains(resourceName))
                {
                    using (var stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }

            throw new ArgumentException("View was not found.", "viewPath");
        }
        #endregion
        #region GetUrlFromViewPath
        /// <summary>
        /// Gets the URL from view path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">viewName</exception>
        public string GetUrlFromViewPath(string path)
        {
            Argument.NullOrEmpty(path, "path");
            Argument.InvalidArgument(!path.StartsWith("~/"), "View path must start with ~.", "viewPath");

            var localViewPath = path.Substring(2);

            Argument.InvalidArgument(!PathUtility.IsFullFileName(localViewPath), "Invalid view path.", "path");

            return localViewPath;
        }
        #endregion
        #region GetViewPathFromUrl
        /// <summary>
        /// Gets the view path from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">viewUrl</exception>
        /// <exception cref="System.ArgumentException">Invalid view url.;viewUrl</exception>
        public string GetViewPathFromUrl(string url)
        {
            Argument.NullOrEmpty(url, "url");
            Argument.InvalidArgument(!PathUtility.IsFullFileName(url), "Invalid url.", "url");

            return "~/" + url.TrimStart('/');
        }
        #endregion

        #endregion
        #endregion
    }
}
