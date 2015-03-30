using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core.Messages
{
    internal class CallMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public JArray Arguments { get; set; }
        public bool InternalMethod { get; set; }
    }
}
