using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Controller provider interface.
    /// </summary>
    public interface IControllerProvider
    {
        /// <summary>
        /// Creates the controller.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        Controller CreateController(string path);
    }
}
