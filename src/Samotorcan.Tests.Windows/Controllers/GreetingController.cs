using Samotorcan.HtmlUi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.Tests.Windows.Controllers
{
    /// <summary>
    /// Greeting controller.
    /// </summary>
    public class GreetingController : Controller
    {
        public string InputString { get; set; }

        public string InputStringPS { get; private set; }

        public string InputStringPG { private get; set; }

        public GreetingController()
        {
            InputString = "Test string";
            InputStringPS = "Other test string";
        }
    }
}
