using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Content provider interface.
    /// </summary>
    public interface IContentProvider
    {
        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        byte[] GetContent(string path);

        /// <summary>
        /// Gets the URL from content path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        string GetUrlFromContentPath(string path);

        /// <summary>
        /// Gets the content path from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        string GetContentPathFromUrl(string url);
    }
}
