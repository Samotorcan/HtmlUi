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
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Controller CreateController(string name, int id);

        /// <summary>
        /// Controller exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        bool ControllerExists(string name);

        /// <summary>
        /// Gets controller types.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Type> GetControllerTypes();
    }
}
