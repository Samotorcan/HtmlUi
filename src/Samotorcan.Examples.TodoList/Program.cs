using Samotorcan.HtmlUi.Core;
using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.OS;

namespace Samotorcan.Examples.TodoList
{
    class Program
    {
        static void Main(string[] args)
        {
            if (HtmlUiRuntime.ApplicationType == ApplicationType.ChildApplication)
            {
                OSChildApplication.Run();
                return;
            }

            OSApplication.Run(new ApplicationContext
            {
                LogSeverity = LogSeverity.Debug
            });
        }
    }
}
