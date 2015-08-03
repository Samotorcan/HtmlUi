using Samotorcan.HtmlUi.Core.Exceptions;
using Samotorcan.HtmlUi.Core.Logs;
using System;

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
        #region LogException
        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        public static TReturn LogException<TReturn>(Func<TReturn> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            try
            {
                return action();
            }
            catch (Exception e)
            {
                Logger.Error("Unhandled exception", e);

                throw;
            }
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="System.ArgumentNullException">action</exception>
        public static void LogException(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            ExceptionUtility.LogException<object>(() =>
            {
                action();

                return null;
            });
        }
        #endregion

        #endregion
        #endregion
    }
}
