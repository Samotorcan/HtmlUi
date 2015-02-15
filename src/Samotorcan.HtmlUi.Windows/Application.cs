using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormsApplication = System.Windows.Forms.Application;

namespace Samotorcan.HtmlUi.Windows
{
    /// <summary>
    /// Application.
    /// </summary>
    [CLSCompliant(false)]
    public class Application : Core.Application
    {
        #region Properties
        #region Internal

        #region Window
        /// <summary>
        /// Gets the window.
        /// </summary>
        /// <value>
        /// The window.
        /// </value>
        internal new Window Window
        {
            get
            {
                return (Window)base.Window;
            }
        }
        #endregion

        #endregion
        #endregion
        #region Methods
        #region Protected

        #region OnInitialize
        /// <summary>
        /// Called when the application is initialized.
        /// </summary>
        protected override void OnInitialize()
        {
            FormsApplication.EnableVisualStyles();
            FormsApplication.SetCompatibleTextRenderingDefault(false);
        }
        #endregion
        #region OnShutdown
        /// <summary>
        /// Called when the application is shutdown.
        /// </summary>
        protected override void OnShutdown()
        {
            FormsApplication.Exit();
        }
        #endregion
        #region RunMessageLoop
        /// <summary>
        /// Runs the message loop.
        /// </summary>
        protected override void RunMessageLoop()
        {
            FormsApplication.Run(Window.Form);
        }
        #endregion
        #region CreateWindow
        /// <summary>
        /// Creates the window.
        /// </summary>
        /// <returns></returns>
        protected override Core.Window CreateWindow()
        {
            return new Window();
        }
        #endregion

        #endregion
        #endregion
    }
}
