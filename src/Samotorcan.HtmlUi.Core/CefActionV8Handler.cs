using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// CEF Action V8 handler.
    /// </summary>
    internal class CefActionV8Handler : CefV8Handler
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
        private CefActionV8HandlerFunction[] Functions { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CefActionV8Handler"/> class.
        /// </summary>
        /// <param name="functions">The functions.</param>
        public CefActionV8Handler(params CefActionV8HandlerFunction[] functions)
        {
            if (functions == null)
                functions = new CefActionV8HandlerFunction[0];

            Functions = functions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CefActionV8Handler"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        public CefActionV8Handler(string name, CefActionV8HandlerFunction.ExecuteAction action)
            : this(new CefActionV8HandlerFunction[] { new CefActionV8HandlerFunction(name, action) }) { }

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
