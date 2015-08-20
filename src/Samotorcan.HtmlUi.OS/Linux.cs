using Samotorcan.HtmlUi.Linux;

namespace Samotorcan.HtmlUi.OS
{
    /// <summary>
    /// Linux only methods. If you call any method in this class the Linux assembly gets loaded.
    /// </summary>
    internal static class Linux
    {
        public static void RunApplication(Core.ApplicationContext settings)
        {
            Application.Run(new ApplicationContext(settings));
        }

        public static void RunChildApplication(Core.ChildApplicationContext settings)
        {
            ChildApplication.Run(new ChildApplicationSettings(settings));
        }
    }
}
