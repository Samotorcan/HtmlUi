using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Utilities
{
    /// <summary>
    /// CEF utility.
    /// </summary>
    [CLSCompliant(false)]
    public static class CefUtility
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

        #endregion
        #endregion
    }
}
