﻿using Samotorcan.HtmlUi.Core;
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
                ((FileAssemblyViewProvider)application.ViewProvider).ViewSearch = ViewSearch.Assembly;

                var view = application.ViewProvider.GetView("~/Views/Index.html");

                application.Window.View = "Index.html";
                application.Window.Borderless = false;

                application.Run();
            }
        }
    }
}