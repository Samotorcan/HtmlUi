using Samotorcan.HtmlUi.Core;
using Samotorcan.HtmlUi.Windows;

namespace Samotorcan.Tests.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            if (HtmlUiRuntime.ApplicationType == ApplicationType.ChildApplication)
            {
                WindowsChildApplication.Run();
                return;
            }

            var settings = new WindowsApplicationSettings();
            settings.CommandLineArgsEnabled = true;
            settings.ChromeViewsEnabled = true;
            settings.WindowSettings.View = "chrome://about";

            WindowsApplication.Run(settings);
        }
    }
}
