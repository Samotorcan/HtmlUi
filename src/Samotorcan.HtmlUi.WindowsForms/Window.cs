using System;
using System.Windows.Forms;
using Xilium.CefGlue;
using Samotorcan.HtmlUi.Core.Utilities;
using Samotorcan.HtmlUi.Core.Renderer;

namespace Samotorcan.HtmlUi.WindowsForms
{
    /// <summary>
    /// Window.
    /// </summary>
    [CLSCompliant(false)]
    public class Window : Core.Window
    {
        #region Events

        #region Resize
        /// <summary>
        /// Occurs on resize.
        /// </summary>
        protected event EventHandler<EventArgs> Resize;
        #endregion

        #endregion
        #region Properties
        #region Internal

        #region Form
        /// <summary>
        /// Gets the form.
        /// </summary>
        /// <value>
        /// The form.
        /// </value>
        internal Form Form { get; private set; }
        #endregion

        #endregion
        #region Private

        #region FormHandle
        /// <summary>
        /// Gets or sets the form handle.
        /// </summary>
        /// <value>
        /// The form handle.
        /// </value>
        private IntPtr FormHandle { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public Window(WindowSettings settings)
            : base(settings)
        {
            Application.Current.InvokeOnUi(() => {
                Form = new Form();

                // default values
                Form.SetBounds(0, 0, 800, 600);

                // events
                Form.HandleCreated += Form_HandleCreated;
                Form.GotFocus += Form_GotFocus;
                Form.Resize += Form_Resize;
                Form.FormClosed += Form_FormClosed;
            });

            KeyPress += Window_KeyPress;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class with default settings.
        /// </summary>
        public Window()
            : this(new WindowSettings()) { }

        #endregion
        #region Methods
        #region Private

        #region Form_HandleCreated
        /// <summary>
        /// Handles the HandleCreated event of the Form control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Form_HandleCreated(object sender, EventArgs e)
        {
            FormHandle = Form.Handle;

            var width = Form.ClientSize.Width;
            var height = Form.ClientSize.Height;

            Application.Current.InvokeOnCef(() =>
            {
                CreateBrowser(FormHandle, new CefRectangle(0, 0, width, height));
            });
        }
        #endregion
        #region Form_Resize
        /// <summary>
        /// Handles the Resize event of the Form control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Form_Resize(object sender, EventArgs e)
        {
            if (Resize != null)
                Resize(this, new EventArgs());
        }
        #endregion
        #region Form_GotFocus
        /// <summary>
        /// Handles the GotFocus event of the Form control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Form_GotFocus(object sender, EventArgs e)
        {
            if (CefBrowser != null)
            {
                CefUtility.ExecuteTask(CefThreadId.UI, () =>
                {
                    CefBrowser.GetHost().SetFocus(true);
                });
            }
        }
        #endregion
        #region Form_FormClosed
        /// <summary>
        /// Handles the FormClosed event of the Form control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FormClosedEventArgs"/> instance containing the event data.</param>
        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Current.InvokeOnMain(() =>
            {
                Application.Current.Shutdown();
            });
        }
        #endregion

        #region Window_KeyPress
        /// <summary>
        /// Handles the KeyPress event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Core.Events.KeyPressEventArgs"/> instance containing the event data.</param>
        private void Window_KeyPress(object sender, Core.Events.KeyPressEventArgs e)
        {
            if (e.KeyEventType == CefKeyEventType.RawKeyDown)
            {
                if (e.NativeKeyCode == (int)Keys.F12)
                    OpenDeveloperTools();
                else if (e.NativeKeyCode == (int)Keys.F5)
                    RefreshView(e.Modifiers == CefEventFlags.ControlDown);
            }
        }
        #endregion

        #region OpenDeveloperTools
        /// <summary>
        /// Opens the developer tools.
        /// </summary>
        private void OpenDeveloperTools()
        {
            Application.Current.InvokeOnMain(() =>
            {
                var windowInfo = CefWindowInfo.Create();
                windowInfo.SetAsPopup(IntPtr.Zero, "Developer tools");
                windowInfo.Width = 1200;
                windowInfo.Height = 500;

                CefBrowser.GetHost().ShowDevTools(windowInfo, new DeveloperToolsClient(), new CefBrowserSettings(), new CefPoint(0, 0));
            });
        }
        #endregion
        #region RefreshView
        /// <summary>
        /// Refreshes the view.
        /// </summary>
        /// <param name="ignoreCache">if set to <c>true</c> ignore cache.</param>
        private void RefreshView(bool ignoreCache)
        {
            if (CefBrowser != null)
            {
                Application.Current.InvokeOnMain(() =>
                {
                    if (ignoreCache)
                        CefBrowser.ReloadIgnoreCache();
                    else
                        CefBrowser.Reload();
                });
            }
        }

        /// <summary>
        /// Refreshes the view.
        /// </summary>
        private void RefreshView()
        {
            RefreshView(false);
        }
        #endregion

        #endregion
        #endregion
        #region IDisposable

        /// <summary>
        /// Was dispose already called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Application.Current.EnsureMainThread();

            if (!_disposed)
            {
                if (disposing)
                {
                    Application.Current.InvokeOnUiAsync(() =>
                    {
                        Form.Dispose();
                    });
                }
            }
        }

        #endregion
    }
}
