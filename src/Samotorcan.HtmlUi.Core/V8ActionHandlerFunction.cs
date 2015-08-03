using System;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// V8 action handler function.
    /// </summary>
    internal class V8ActionHandlerFunction
    {
        #region ExecuteAction
        /// <summary>
        /// Excecute action delegate.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="obj">The object.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="returnValue">The return value.</param>
        /// <param name="exception">The exception.</param>
        public delegate void ExecuteAction(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception);
        #endregion

        #region Properties
        #region Public

        #region Name
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }
        #endregion
        #region Action
        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public ExecuteAction Action { get; private set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="V8ActionHandlerFunction"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="System.ArgumentNullException">
        /// name
        /// or
        /// action
        /// </exception>
        public V8ActionHandlerFunction(string name, ExecuteAction action)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            if (action == null)
                throw new ArgumentNullException("action");

            Name = name;
            Action = action;
        }

        #endregion
    }
}
