using Samotorcan.HtmlUi.Core;

namespace Samotorcan.Tests.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            if (HtmlUiRuntime.ApplicationType == ApplicationType.ChildApplication)
            {
                HtmlUi.Windows.ChildApplication.Run();
                return;
            }

            var settings = new HtmlUi.Windows.ApplicationContext();
            settings.CommandLineArgsEnabled = true;
            settings.ChromeViewsEnabled = true;
            settings.WindowSettings.View = "chrome://about";

            HtmlUi.Windows.Application.Run(settings);
        }
    }
}
