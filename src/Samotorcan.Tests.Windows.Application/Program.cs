using Samotorcan.HtmlUi.Core;
using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.Tests.Windows.Application
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
            var settings = new MainApplicationSettings();
            settings.CommandLineArgsEnabled = true;
            settings.ChromeViewsEnabled = true;
            settings.WindowSettings.View = "chrome://about";

            using (var application = new MainApplication(settings))
            {
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
