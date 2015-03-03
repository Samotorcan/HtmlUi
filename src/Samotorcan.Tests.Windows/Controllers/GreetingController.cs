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
        private string _inputString;
        public string InputString
        {
            get
            {
                return _inputString;
            }
            set
            {
                _inputString = value;
            }
        }

        public string InputStringPS { get; private set; }

        public string InputStringPG { private get; set; }

        public GreetingController(int id)
            : base(id)
        {
            InputString = "Test string";
            InputStringPS = "Other test string";
        }

        public void TestMethod1()
        {
            
        }

        public void TestMethod2(int a)
        {

        }

        public string TestMethod2(int b, string z)
        {
            return string.Empty;
        }
    }
}
