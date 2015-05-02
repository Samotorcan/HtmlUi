using Samotorcan.HtmlUi.Core.Logs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Utilities
{
    /// <summary>
    /// CEF utility.
    /// </summary>
    internal static class CefUtility
    {
        #region Methods
        #region Public

        #region ExecuteTask
        /// <summary>
        /// Executes the task on the selected CEF thread.
        /// </summary>
        /// <param name="threadId">The thread identifier.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="System.ArgumentNullException">action</exception>
        public static void ExecuteTask(CefThreadId threadId, Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (!CefRuntime.CurrentlyOn(threadId))
                CefRuntime.PostTask(threadId, new ActionTask(action));
            else
                action();
        }
        #endregion
        #region RunInContext
        /// <summary>
        /// Run in context.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// context
        /// or
        /// action
        /// </exception>
        public static TReturn RunInContext<TReturn>(CefV8Context context, Func<TReturn> action)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (action == null)
                throw new ArgumentNullException("action");

            var contextEntered = false;
            if (!CefV8Context.InContext || !CefV8Context.GetEnteredContext().IsSame(context))
            {
                context.Enter();
                contextEntered = true;
            }

            try
            {
                return action();
            }
            finally
            {
                if (contextEntered)
                {
                    context.Exit();
                    contextEntered = false;
                }
            }
        }

        /// <summary>
        /// Run in context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="System.ArgumentNullException">
        /// context
        /// or
        /// action
        /// </exception>
        public static void RunInContext(CefV8Context context, Action action)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (action == null)
                throw new ArgumentNullException("action");

            CefUtility.RunInContext<object>(context, () =>
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
