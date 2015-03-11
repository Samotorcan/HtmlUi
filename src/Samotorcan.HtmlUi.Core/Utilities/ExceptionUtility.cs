using Samotorcan.HtmlUi.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core.Utilities
{
    /// <summary>
    /// Exception utility.
    /// </summary>
    internal static class ExceptionUtility
    {
        #region Methods
        #region Public

        #region CreateJavascriptException
        /// <summary>
        /// Creates the javascript exception.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">e</exception>
        public static JavascriptException CreateJavascriptException(Exception e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            // squash aggregate exception
            var aggregateException = e as AggregateException;
            if (aggregateException != null)
            {
                if (aggregateException.InnerExceptions.Count == 1)
                    return CreateJavascriptException(aggregateException.InnerExceptions[0]);
            }

            // native exception
            var nativeException = e as INativeException;
            if (nativeException != null)
                return nativeException.ToJavascriptException();

            // normal exception
            return new JavascriptException
            {
                Type = "Exception",
                Message = e.Message,
                InnerException = e.InnerException != null ? CreateJavascriptException(e.InnerException) : null
            };
        }
        #endregion

        #endregion
        #endregion
    }
}
