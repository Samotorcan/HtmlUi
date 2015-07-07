using Samotorcan.HtmlUi.Core;
using Samotorcan.HtmlUi.Linux;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.Tests.Linux
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
