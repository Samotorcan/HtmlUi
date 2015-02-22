using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Utilities
{
    /// <summary>
    /// Message utility.
    /// </summary>
    internal class MessageUtility
    {
        #region Properties
        #region Public

        #region SendBinaryMessage
        /// <summary>
        /// Sends the binary message.
        /// </summary>
        /// <param name="cefBrowser">The cef browser.</param>
        /// <param name="name">The name.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">cefBrowser</exception>
        public static void SendBinaryMessage(CefBrowser cefBrowser, string name, byte[] message)
        {
            if (cefBrowser == null)
                throw new ArgumentNullException("cefBrowser");

            if (message == null)
                message = new byte[0];

            using (var processMessage = CefProcessMessage.Create(name))
            {
                try
                {
                    using (var binaryValue = CefBinaryValue.Create(message))
                    {
                        processMessage.Arguments.SetBinary(0, binaryValue);

                        cefBrowser.SendProcessMessage(CefProcessId.Browser, processMessage);
                    }
                }
                finally
                {
                    processMessage.Arguments.Dispose();
                }
            }
        }
        #endregion

        #endregion
        #endregion
    }
}
