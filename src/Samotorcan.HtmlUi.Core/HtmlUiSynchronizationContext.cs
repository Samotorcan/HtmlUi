using Samotorcan.HtmlUi.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// HtmlUi synchronization context.
    /// </summary>
    public class HtmlUiSynchronizationContext : SynchronizationContext
    {
        #region Methods
        #region Public

        #region Post
        /// <summary>
        /// Dispatches an asynchronous message to a synchronization context.
        /// </summary>
        /// <param name="d">The <see cref="T:System.Threading.SendOrPostCallback" /> delegate to call.</param>
        /// <param name="state">The object passed to the delegate.</param>
        /// <exception cref="System.ArgumentNullException">d</exception>
        public override void Post(SendOrPostCallback d, object state)
        {
            Argument.Null(d, "d");

            BaseApplication.Current.InvokeOnMainAsync(() =>
            {
                d.Invoke(state);
            });
        }
        #endregion
        #region Send
        /// <summary>
        /// Dispatches a synchronous message to a synchronization context.
        /// </summary>
        /// <param name="d">The <see cref="T:System.Threading.SendOrPostCallback" /> delegate to call.</param>
        /// <param name="state">The object passed to the delegate.</param>
        /// <exception cref="System.ArgumentNullException">d</exception>
        public override void Send(SendOrPostCallback d, object state)
        {
            Argument.Null(d, "d");

            BaseApplication.Current.InvokeOnMain(() =>
            {
                d.Invoke(state);
            });
        }
        #endregion

        #endregion
        #endregion
    }
}
