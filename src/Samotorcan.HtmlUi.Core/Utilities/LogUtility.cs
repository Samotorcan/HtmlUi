using Samotorcan.HtmlUi.Core.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core.Utilities
{
    /// <summary>
    /// Log utility.
    /// </summary>
    internal static class LogUtility
    {
        #region Methods
        #region Public

        #region LogUtility
        /// <summary>
        /// Logs the action run.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// start
        /// or
        /// end
        /// or
        /// action
        /// </exception>
        public static TReturn Log<TReturn>(string start, string end, Func<TReturn> action)
        {
            if (string.IsNullOrWhiteSpace(start))
                throw new ArgumentNullException("start");

            if (string.IsNullOrWhiteSpace(end))
                throw new ArgumentNullException("end");

            if (action == null)
                throw new ArgumentNullException("action");

            GeneralLog.Debug(start);

            try
            {
                return ExceptionUtility.LogException<TReturn>(action);
            }
            finally
            {
                GeneralLog.Debug(end);
            }
        }

        /// <summary>
        /// Logs the action run.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="System.ArgumentNullException">
        /// start
        /// or
        /// end
        /// or
        /// action
        /// </exception>
        public static void Log(string start, string end, Action action)
        {
            if (string.IsNullOrWhiteSpace(start))
                throw new ArgumentNullException("start");

            if (string.IsNullOrWhiteSpace(end))
                throw new ArgumentNullException("end");

            if (action == null)
                throw new ArgumentNullException("action");

            LogUtility.Log<object>(start, end, () =>
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
