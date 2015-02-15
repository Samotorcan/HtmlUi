using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core.Handlers
{
    /// <summary>
    /// Default browser process handler.
    /// </summary>
    [CLSCompliant(false)]
    public class DefaultBrowserProcessHandler : CefBrowserProcessHandler
    {
        #region Methods
        #region Protected

        #region OnBeforeChildProcessLaunch
        /// <summary>
        /// Called before child process launch.
        /// </summary>
        /// <param name="commandLine">The command line.</param>
        protected override void OnBeforeChildProcessLaunch(CefCommandLine commandLine)
        {
            // .NET in Windows treat assemblies as native images, so no any magic required.
            // Mono on any platform usually located far away from entry assembly, so we want prepare command line to call it correctly.
            if (Type.GetType("Mono.Runtime") != null)
            {
                if (!commandLine.HasSwitch("cefglue"))
                {
                    var path = PathUtility.Application;
                    commandLine.SetProgram(path);

                    var mono = CefRuntime.Platform == CefRuntimePlatform.Linux ? "/usr/bin/mono" : @"C:\Program Files\Mono-2.10.8\bin\monow.exe";
                    commandLine.PrependArgument(mono);

                    commandLine.AppendSwitch("cefglue", "w");
                }
            }

            if (!Application.Current.EnableD3D11)
                commandLine.AppendArgument(CefArgument.DisableD3D11.Value);
        }
        #endregion

        #endregion
        #endregion
    }
}
