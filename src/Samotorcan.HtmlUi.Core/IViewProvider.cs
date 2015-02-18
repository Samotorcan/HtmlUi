using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// View provider interface.
    /// </summary>
    public interface IViewProvider
    {
        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <param name="viewPath">The view path.</param>
        /// <returns></returns>
        string GetView(string viewPath);

        /// <summary>
        /// Gets the URL from view path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        string GetUrlFromViewPath(string path);

        /// <summary>
        /// Gets the view path from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        string GetViewPathFromUrl(string url);
    }
}
