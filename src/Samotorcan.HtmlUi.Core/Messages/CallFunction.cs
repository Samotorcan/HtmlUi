using System;

namespace Samotorcan.HtmlUi.Core.Messages
{
    internal class CallFunction
    {
        public Guid CallbackId { get; set; }
        public string Name { get; set; }
        public object Data { get; set; }
    }
}
