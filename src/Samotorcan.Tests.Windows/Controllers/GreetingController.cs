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
        public int FirstNumber { get; set; }


        private int _secondNumber;
        public int SecondNumber
        {
            get { return _secondNumber; }
            set { SetField(ref _secondNumber, value); }
        }

        private int _result;
        public int Result
        {
            get { return _result; }
            set { SetField(ref _result, value); }
        }

        public GreetingController(int id)
            : base(id)
        {
            
        }

        public void Sum()
        {
            SecondNumber += 10;
            Result = FirstNumber + SecondNumber;
        }

        public void Sum1()
        {
            
        }
    }
}
