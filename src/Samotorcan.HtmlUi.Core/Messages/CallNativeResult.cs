using System;

namespace Samotorcan.HtmlUi.Core.Messages
{
    internal class CallNativeResult
    {
        public Guid? CallbackId { get; set; }
        public string JsonResult { get; set; }
    }
}
