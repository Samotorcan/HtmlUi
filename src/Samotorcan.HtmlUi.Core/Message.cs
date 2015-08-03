using System;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Message.
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    internal class Message<TData>
    {
        #region Properties
        #region Public

        #region CallbackId
        /// <summary>
        /// Gets or sets the callback identifier.
        /// </summary>
        /// <value>
        /// The callback identifier.
        /// </value>
        public Guid? CallbackId { get; protected set; }
        #endregion
        #region Data
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public TData Data { get; protected set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Message{TData}"/> class.
        /// </summary>
        /// <param name="callbackId">The callback identifier.</param>
        /// <param name="data">The data.</param>
        public Message(Guid? callbackId, TData data)
        {
            CallbackId = callbackId;
            Data = data;
        }

        #endregion
    }
}
