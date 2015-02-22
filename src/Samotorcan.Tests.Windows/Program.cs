using Samotorcan.HtmlUi.Core;
using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.Tests.Windows
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var application = new Application())
            {
                var controller = application.ControllerProvider.CreateController("GreetingController");

                application.Run();
            }
        }
    }
}
