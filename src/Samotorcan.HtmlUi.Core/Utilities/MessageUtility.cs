﻿using System;
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

        #region SendMessage
        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="cefBrowser">The cef browser.</param>
        /// <param name="name">The name.</param>
        /// <param name="data">The data.</param>
        /// <exception cref="System.ArgumentNullException">
        /// cefBrowser
        /// or
        /// name
        /// </exception>
        public static void SendMessage(CefProcessId process, CefBrowser cefBrowser, string name, object data)
        {
            if (cefBrowser == null)
                throw new ArgumentNullException("cefBrowser");

            if (string.IsNullOrWhiteSpace("name"))
                throw new ArgumentNullException("name");

            var message = JsonUtility.SerializeToBson(data);

            SendBinaryMessage(process, cefBrowser, name, message);
        }
        #endregion
        #region DeserializeMessage
        /// <summary>
        /// Deserializes the message.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="processMessage">The process message.</param>
        /// <returns></returns>
        public static TType DeserializeMessage<TType>(CefProcessMessage processMessage)
        {
            return JsonUtility.DeserializeFromBson<TType>(processMessage.Arguments.GetBinary(0).ToArray());
        }

        /// <summary>
        /// Deserializes the message.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="processMessage">The process message.</param>
        /// <param name="anonymousObject">The anonymous object.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "anonymousObject", Justification = "The type for anonymous object.")]
        public static TType DeserializeMessage<TType>(CefProcessMessage processMessage, TType anonymousObject)
        {
            return JsonUtility.DeserializeFromBson<TType>(processMessage.Arguments.GetBinary(0).ToArray());
        }
        #endregion

        #endregion
        #region Private

        #region SendBinaryMessage
        /// <summary>
        /// Sends the binary message.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="cefBrowser">The cef browser.</param>
        /// <param name="name">The name.</param>
        /// <param name="message">The message.</param>
        private static void SendBinaryMessage(CefProcessId process, CefBrowser cefBrowser, string name, byte[] message)
        {
            using (var processMessage = CefProcessMessage.Create(name))
            {
                try
                {
                    using (var binaryValue = CefBinaryValue.Create(message))
                    {
                        processMessage.Arguments.SetBinary(0, binaryValue);

                        cefBrowser.SendProcessMessage(process, processMessage);
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
