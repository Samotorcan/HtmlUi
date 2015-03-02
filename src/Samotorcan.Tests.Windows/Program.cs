using Samotorcan.HtmlUi.Core;
using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.Tests.Windows
{
    class Program
    {
        static void Main(string[] args)
        {
            if (HtmlUiRuntime.ApplicationType == ApplicationType.MainApplication)
                RunMainApplication();
            else
                RunChildApplication();
        }

        private static void RunMainApplication()
        {
            using (var application = new MainApplication())
            {
                var controller = application.ControllerProvider.CreateController("GreetingController", 12);

                application.Run();
            }
        }

        private static void RunChildApplication()
        {
            using (var application = new ChildApplication())
            {
                application.Run();
            }
        }
    }
}
