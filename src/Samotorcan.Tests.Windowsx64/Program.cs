using Samotorcan.HtmlUi.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.Tests.Windowsx64
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var application = new Application())
            {
                application.Window.View = "~/Views/Index.html";
                application.Window.Borderless = false;

                application.Run();
            }
        }
    }
}
