using Samotorcan.HtmlUi.Windows;

namespace Samotorcan.HtmlUi.OS
{
    /// <summary>
    /// Windows only methods. If you call any method in this class the Windows assembly gets loaded.
    /// </summary>
    internal static class Windows
    {
        public static void RunApplication(Core.ApplicationContext settings)
        {
            Application.Run(new ApplicationContext(settings));
        }

        public static void RunChildApplication(Core.ChildApplicationContext settings)
        {
            ChildApplication.Run(new ChildApplicationContext(settings));
        }
    }
}
