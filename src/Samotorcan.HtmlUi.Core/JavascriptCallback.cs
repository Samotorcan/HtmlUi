using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Javascript callback.
    /// </summary>
    internal class JavascriptCallback
    {
        #region Properties
        #region Public

        #region Id
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; private set; }
        #endregion
        #region CallbackFunction
        /// <summary>
        /// Gets the callback function.
        /// </summary>
        /// <value>
        /// The callback function.
        /// </value>
        public CefV8Value CallbackFunction { get; private set; }
        #endregion
        #region Context
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public CefV8Context Context { get; private set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JavascriptCallback"/> class.
        /// </summary>
        /// <param name="callbackFunction">The callback function.</param>
        /// <param name="context">The context.</param>
        /// <exception cref="System.ArgumentNullException">
        /// callbackFunction
        /// or
        /// context
        /// </exception>
        public JavascriptCallback(CefV8Value callbackFunction, CefV8Context context)
        {
            if (callbackFunction == null)
                throw new ArgumentNullException("callbackFunction");

            if (context == null)
                throw new ArgumentNullException("context");

            Id = Guid.NewGuid();
            CallbackFunction = callbackFunction;
            Context = context;
        }

        #endregion
        #region Methods
        #region Public

        #region Execute
        /// <summary>
        /// Executes the callback.
        /// </summary>
        public void Execute()
        {
            CallbackFunction.ExecuteFunctionWithContext(Context, null, new CefV8Value[0]);
        }

        /// <summary>
        /// Executes the callback with data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Execute(object data)
        {
            CallbackFunction.ExecuteFunctionWithContext(Context, null, new CefV8Value[] { CefV8Value.CreateString(JsonConvert.SerializeObject(data)) });
        }
        #endregion

        #endregion
        #endregion
    }
}
