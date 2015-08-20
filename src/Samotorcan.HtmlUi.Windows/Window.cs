using System;
using System.Windows.Forms;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Windows
{
    /// <summary>
    /// Windows window.
    /// </summary>
    [CLSCompliant(false)]
    public class Window : WindowsForms.Window
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public Window(WindowContext settings)
            : base(settings)
        {
            Resize += Window_Resize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class with default settings.
        /// </summary>
        public Window()
            : this(new WindowContext()) { }

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
                if (Form.WindowState == FormWindowState.Minimized)
                {
                    // hide the browser
                    NativeMethods.SetWindowPos(CefBrowser.GetHost().GetWindowHandle(), IntPtr.Zero, 0, 0, 0, 0,
                        NativeMethods.SetWindowPosFlags.NoMove | NativeMethods.SetWindowPosFlags.NoZOrder | NativeMethods.SetWindowPosFlags.NoActivate);
                }
                else
                {
                    // resize the browser
                    var width = Form.ClientSize.Width;
                    var height = Form.ClientSize.Height;

                    NativeMethods.SetWindowPos(CefBrowser.GetHost().GetWindowHandle(), IntPtr.Zero, 0, 0, width, height,
                        NativeMethods.SetWindowPosFlags.NoMove | NativeMethods.SetWindowPosFlags.NoZOrder);
                }
            }
        }
        #endregion

        #endregion
        #endregion
    }
}
