using Samotorcan.HtmlUi.Core.Handlers.Browser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xilium.CefGlue;
using System.ComponentModel;
using System.Drawing;
using Samotorcan.HtmlUi.Core.Utilities;
using System.Runtime.InteropServices;
using Samotorcan.HtmlUi.Core.Events;

namespace Samotorcan.HtmlUi.Windows
{
    /// <summary>
    /// Window.
    /// </summary>
    [CLSCompliant(false)]
    public class Window : Core.BaseWindow
    {
        #region Properties
        #region Public

        #region Borderless
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Window" /> is borderless.
        /// </summary>
        /// <value>
        ///   <c>true</c> if borderless; otherwise, <c>false</c>.
        /// </value>
        public override bool Borderless
        {
            get
            {
                return base.Borderless;
            }
            set
            {
                base.Borderless = value;

                if (IsBrowserCreated)
                    SyncFormBorderlessProperty();
            }
        }
        #endregion

        #endregion
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
        #region MouseLeftButtonDown
        /// <summary>
        /// Gets or sets a value indicating whether left mouse button is down.
        /// </summary>
        /// <value>
        /// <c>true</c> if left mouse button is down; otherwise, <c>false</c>.
        /// </value>
        private bool MouseLeftButtonDown { get; set; }
        #endregion
        #region MousePosition
        /// <summary>
        /// Gets or sets the mouse position.
        /// </summary>
        /// <value>
        /// The mouse position.
        /// </value>
        private Point MousePosition { get; set; }
        #endregion
        #region Browser_MouseEventDelegate
        /// <summary>
        /// Gets or sets the browser mouse event delegate.
        /// </summary>
        /// <value>
        /// The browser mouse event delegate.
        /// </value>
        private NativeMethods.HookProc Browser_MouseEventDelegate { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        public Window()
            : base()
        {
            Application.Current.InvokeOnUi(() => {
                Form = new Form();

                // default values
                Form.SetBounds(0, 0, 800, 600);

                // events
                Form.HandleCreated += Form_HandleCreated;
                Form.GotFocus += Form_GotFocus;
                Form.Move += Form_Move;
                Form.Resize += Form_Resize;
                Form.FormClosed += Form_FormClosed;
                Form.ProcessWndProc = Form_ProcessWndProc;
            });

            BrowserCreated += Window_BrowserCreated;
            Browser_MouseEventDelegate = new NativeMethods.HookProc(Browser_MouseEvent);
        }

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

            SyncFormBorderlessProperty();

            var width = Form.ClientSize.Width;
            var height = Form.ClientSize.Height;

            Application.Current.InvokeOnMain(() => {
                CreateBrowser(FormHandle, new CefRectangle(0, 0, width, height));
            });

            SetSystemContextMenu();
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
                    var width = Borderless ? Form.Width : Form.ClientSize.Width;
                    var height = Borderless ? Form.Height : Form.ClientSize.Height;

                    NativeMethods.SetWindowPos(CefBrowser.GetHost().GetWindowHandle(), IntPtr.Zero, 0, 0, width, height,
                        NativeMethods.SetWindowPosFlags.NoMove | NativeMethods.SetWindowPosFlags.NoZOrder);
                }
            }
        }
        #endregion
        #region Form_Move
        /// <summary>
        /// Handles the Move event of the Form control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Form_Move(object sender, EventArgs e)
        {
            if (CefBrowser != null)
            {
                CefUtility.ExecuteTask(CefThreadId.UI, () =>
                {
                    CefBrowser.GetHost().NotifyMoveOrResizeStarted();
                });
            }
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
            Form.Dispose();

            Application.Current.InvokeOnMain(() =>
            {
                Dispose();
                Application.Current.Shutdown();
            });
        }
        #endregion
        #region Form_ProcessWndProc
        /// <summary>
        /// Process the form WND proc.
        /// </summary>
        /// <param name="m">The m.</param>
        private void Form_ProcessWndProc(ref Message m)
        {
            // developer tools
            if ((m.Msg == NativeMethods.WM_SYSCOMMAND) && ((int)m.WParam == NativeMethods.SYSMENU_DEVTOOLS_ID))
            {
                Application.Current.InvokeOnMain(() => {
                    var windowInfo = CefWindowInfo.Create();
                    windowInfo.SetAsPopup(IntPtr.Zero, "Developer tools");
                    windowInfo.Width = 1200;
                    windowInfo.Height = 500;

                    CefBrowser.GetHost().ShowDevTools(windowInfo, new DeveloperToolsCefClient(), new CefBrowserSettings(), new CefPoint(0, 0));
                });
            }
        }
        #endregion

        #region Window_BrowserCreated
        /// <summary>
        /// Handles the browser created event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Window_BrowserCreated(object sender, BrowserCreatedEventArgs e)
        {
            // hook mouse events
            var threadId = NativeMethods.GetWindowThreadProcessId(CefBrowser.GetHost().GetWindowHandle(), IntPtr.Zero);
            NativeMethods.SetWindowsHookEx(NativeMethods.HookType.WH_MOUSE, Browser_MouseEventDelegate, IntPtr.Zero, threadId);
        }
        #endregion
        #region Browser_MouseEvent
        /// <summary>
        /// Handle the browser mouse events.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="wParam">The w parameter.</param>
        /// <param name="lParam">The l parameter.</param>
        /// <returns></returns>
        private IntPtr Browser_MouseEvent(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code == 0)
            {
                var data = (NativeMethods.MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(NativeMethods.MouseHookStruct));
                var mouseMessage = (NativeMethods.Messages)wParam;

                if (mouseMessage == NativeMethods.Messages.WM_MOUSEMOVE)
                {
                    // move borderless window
                    if (Borderless && MouseLeftButtonDown)
                    {
                        var deltaX = data.pt.x - MousePosition.X;
                        var deltaY = data.pt.y - MousePosition.Y;

                        var windowPosition = new NativeMethods.RECT();
                        NativeMethods.GetWindowRect(FormHandle, out windowPosition);

                        NativeMethods.SetWindowPos(FormHandle, IntPtr.Zero,
                            windowPosition.Left + deltaX, windowPosition.Top + deltaY, 0, 0,
                            NativeMethods.SetWindowPosFlags.NoSize | NativeMethods.SetWindowPosFlags.NoZOrder);
                    }
                }
                else if (mouseMessage == NativeMethods.Messages.WM_LBUTTONDOWN)
                {
                    MouseLeftButtonDown = true;
                }
                else if (mouseMessage == NativeMethods.Messages.WM_LBUTTONUP)
                {
                    MouseLeftButtonDown = false;
                }

                MousePosition = new Point(data.pt.x, data.pt.y);
            }

            return NativeMethods.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }
        #endregion

        #region SyncFormBorderlessProperty
        /// <summary>
        /// Synchronizes the form borderless property.
        /// </summary>
        private void SyncFormBorderlessProperty()
        {
            Form.FormBorderStyle = Borderless ? FormBorderStyle.None : FormBorderStyle.Sizable;
        }
        #endregion
        #region SetSystemContextMenu
        /// <summary>
        /// Sets the system context menu.
        /// </summary>
        private void SetSystemContextMenu()
        {
            IntPtr hSysMenu = NativeMethods.GetSystemMenu(FormHandle, false);

            NativeMethods.AppendMenu(hSysMenu, NativeMethods.MF_SEPARATOR, 0, string.Empty);
            NativeMethods.AppendMenu(hSysMenu, NativeMethods.MF_STRING, NativeMethods.SYSMENU_DEVTOOLS_ID, "Developer Tools");
        }
        #endregion

        #endregion
        #endregion
    }
}
