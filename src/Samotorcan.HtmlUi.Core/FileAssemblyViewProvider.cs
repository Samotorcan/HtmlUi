﻿using Samotorcan.HtmlUi.Core.Utilities;
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
        /// <param name="viewPath">Name of the view.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">viewName</exception>
        /// <exception cref="System.ArgumentException">
        /// View name must start with ~/.;viewName
        /// or
        /// Invalid view name.;viewName
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "It's ok.")]
        public string GetView(string viewPath)
        {
            if (string.IsNullOrEmpty(viewPath))
                throw new ArgumentNullException("viewPath");

            var absoluteFullViewPath = FileAssemblyViewProvider.GetAbsoluteFullViewPath(viewPath);
            var localViewPath = absoluteFullViewPath.Substring(1);

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
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            return FileAssemblyViewProvider.GetRelativeFullViewPath(path);
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
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            return FileAssemblyViewProvider.GetAbsoluteFullViewPath(url);
        }
        #endregion
        #region GetAbsoluteFullViewPath
        /// <summary>
        /// Gets the absolute full view path.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">viewName</exception>
        /// <exception cref="System.ArgumentException">Invalid view name.;viewName</exception>
        public static string GetAbsoluteFullViewPath(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
                throw new ArgumentNullException("viewName");

            if (!viewName.StartsWith("~/Views/"))
            {
                if (viewName.StartsWith("~"))
                    throw new ArgumentException("Views must located in the Views directory.", "viewName");

                viewName = "~/Views/" + viewName;
            }

            if (!PathUtility.IsFullFileName(viewName.Substring(1)))
                throw new ArgumentException("Invalid view name.", "viewName");

            return viewName;
        }
        #endregion
        #region GetRelativeFullViewPath
        /// <summary>
        /// Gets the relative full view path.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">viewName</exception>
        /// <exception cref="System.ArgumentException">Invalid view name.;viewName</exception>
        public static string GetRelativeFullViewPath(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
                throw new ArgumentNullException("viewName");

            if (viewName.StartsWith("~/Views/"))
                viewName = viewName.Substring(8);

            if (viewName.StartsWith("~"))
                throw new ArgumentException("Views must located in the Views directory.", "viewName");

            if (!PathUtility.IsFullFileName(viewName))
                throw new ArgumentException("Invalid view name.", "viewName");

            return viewName;
        }
        #endregion

        #endregion
        #endregion
    }
}