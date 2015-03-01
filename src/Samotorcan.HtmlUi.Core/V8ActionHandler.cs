using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// V8 action handler.
    /// </summary>
    internal class V8ActionHandler : CefV8Handler
    {
        #region Properties
        #region Private

        #region Functions
        /// <summary>
        /// Gets or sets the functions.
        /// </summary>
        /// <value>
        /// The functions.
        /// </value>
        private V8ActionHandlerFunction[] Functions { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="V8ActionHandler"/> class.
        /// </summary>
        /// <param name="functions">The functions.</param>
        public V8ActionHandler(params V8ActionHandlerFunction[] functions)
        {
            if (functions == null)
                functions = new V8ActionHandlerFunction[0];

            Functions = functions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="V8ActionHandler"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        public V8ActionHandler(string name, V8ActionHandlerFunction.ExecuteAction action)
            : this(new V8ActionHandlerFunction[] { new V8ActionHandlerFunction(name, action) }) { }

        #endregion
        #region Methods
        #region Protected

        #region Execute
        /// <summary>
        /// Excecute the function in Functions if found.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        /// <param name="arguments"></param>
        /// <param name="returnValue"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            returnValue = null;
            exception = null;

            var function = Functions.FirstOrDefault(f => f.Name == name);
            if (function != null)
            {
                function.Action(name, obj, arguments, out returnValue, out exception);
                return true;
            }

            return false;
        }
        #endregion

        #endregion
        #endregion
    }
}
