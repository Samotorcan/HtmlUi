using System;

namespace Samotorcan.HtmlUi.Core.Metadata
{
    internal class SyncProperty
    {
        public string Name { get; set; }
        public Delegate GetDelegate { get; set; }
        public Delegate SetDelegate { get; set; }
        public Type SyncPropertyType { get; set; }
    }
}
