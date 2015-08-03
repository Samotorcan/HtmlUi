using System;
using System.Collections.Generic;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Controller provider interface.
    /// </summary>
    public interface IControllerProvider
    {
        /// <summary>
        /// Gets controller types.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Type> ControllerTypes { get; }

        /// <summary>
        /// Creates the controller.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Controller CreateController(string name, int id);

        /// <summary>
        /// Creates the observable controller.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        ObservableController CreateObservableController(string name, int id);

        /// <summary>
        /// Controller exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        bool ControllerExists(string name);
    }
}
