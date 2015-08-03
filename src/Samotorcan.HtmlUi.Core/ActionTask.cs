using System;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Action task.
    /// </summary>
    [CLSCompliant(false)]
    public class ActionTask : CefTask
    {
        #region Properties
        #region Private

        #region Action
        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        private Action Action { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionTask"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public ActionTask(Action action)
            : base()
        {
            Action = action;
        }

        #endregion
        #region Methods
        #region Protected

        #region Execute
        /// <summary>
        /// Executes the action.
        /// </summary>
        protected override void Execute()
        {
            if (Action != null)
                Action();
        }
        #endregion

        #endregion
        #endregion
    }
}
