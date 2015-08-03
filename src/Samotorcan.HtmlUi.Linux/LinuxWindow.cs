using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Windows.Forms;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Linux
{
    /// <summary>
    /// Linux window
    /// </summary>
    [CLSCompliant(false)]
    public class LinuxWindow : WindowsForms.Window
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LinuxWindow"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public LinuxWindow(LinuxWindowSettings settings)
            : base(settings)
        {
            Resize += Window_Resize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinuxWindow"/> class with default settings.
        /// </summary>
        public LinuxWindow()
            : this(new LinuxWindowSettings()) { }

        #endregion
        #region Methods
        #region Private

        #region Window_Resize
        /// <summary>
        /// Handles the Resize event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Window_Resize(object sender, EventArgs e)
        {
            if (CefBrowser != null)
            {
                CefUtility.ExecuteTask(CefThreadId.UI, () =>
                {
                    var display = NativeMethods.get_xdisplay();

                    if (Form.WindowState != FormWindowState.Minimized)
                    {
                        // resize the browser
                        NativeMethods.XResizeWindow(display, CefBrowser.GetHost().GetWindowHandle(), (uint)Form.ClientSize.Width, (uint)Form.ClientSize.Height);
                    }
                });
            }
        }
        #endregion

        #endregion
        #endregion
    }
}
