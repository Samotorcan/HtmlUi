using Samotorcan.HtmlUi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Linux
{
    /// <summary>
    /// Main application.
    /// </summary>
    [CLSCompliant(false)]
    public class MainApplication : BaseMainApplication
    {
        #region Properties
        #region Public

        #region Current
        /// <summary>
        /// Gets the current application.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        public static new MainApplication Current
        {
            get
            {
                return (MainApplication)Core.BaseMainApplication.Current;
            }
        }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainApplication"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public MainApplication(MainApplicationSettings settings)
            : base(settings)
        {
            var display = NativeMethods.XOpenDisplay(null);
            if (display == IntPtr.Zero)
                throw new InvalidOperationException("Display couldn't be opend.");

            var screenNumber = NativeMethods.XDefaultScreen(display);
            var rootWindow = NativeMethods.XDefaultRootWindow(display);
            var blackPixel = NativeMethods.XBlackPixel(display, screenNumber);
            var whitePixel = NativeMethods.XWhitePixel(display, screenNumber);

            var window = NativeMethods.XCreateSimpleWindow(display, rootWindow, 10, 10, 100, 100, 1, blackPixel, whitePixel);

            NativeMethods.XSelectInput(display, window, NativeMethods.EventMask.ExposureMask | NativeMethods.EventMask.KeyPressMask);
            NativeMethods.XMapWindow(display, window);

            var xEvent = new NativeMethods.XEvent();
            while (true)
            {
                NativeMethods.XNextEvent(display, ref xEvent);

                if (xEvent.type == NativeMethods.XEventName.KeyPress)
                    break;
            }

            NativeMethods.XCloseDisplay(display);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainApplication"/> class.
        /// </summary>
        public MainApplication()
            : this(new MainApplicationSettings()) { }

        #endregion
    }
}
