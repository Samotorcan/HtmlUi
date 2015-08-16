using System;
using Newtonsoft.Json.Linq;

namespace Samotorcan.HtmlUi.Core.Messages
{
    internal class CallFunctionResult
    {
        public Guid CallbackId { get; set; }
        public JToken Result { get; set; }
    }
}
