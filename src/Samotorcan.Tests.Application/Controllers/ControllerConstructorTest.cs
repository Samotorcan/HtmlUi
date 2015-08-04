using Samotorcan.HtmlUi.Core;
using System.Collections.Generic;

namespace Samotorcan.Tests.Application.Controllers
{
    /// <summary>
    /// Controller constructor test.
    /// </summary>
    public class ControllerConstructorTest : Controller
    {
        /// <summary>
        /// Void no arguments.
        /// </summary>
        public void VoidNoArguments() { }

        /// <summary>
        /// Void arguments.
        /// </summary>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <param name="arg3">The arg3.</param>
        /// <param name="arg4">The arg4.</param>
        /// <param name="arg5">The arg5.</param>
        public void VoidArguments(string arg1, int arg2, int? arg3, List<string> arg4, ArgObject arg5) { }

        /// <summary>
        /// Return no arguments.
        /// </summary>
        /// <returns></returns>
        public int ReturnNoArguments()
        {
            return 123;
        }

        /// <summary>
        /// Return arguments.
        /// </summary>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <param name="arg3">The arg3.</param>
        /// <param name="arg4">The arg4.</param>
        /// <param name="arg5">The arg5.</param>
        /// <returns></returns>
        public string ReturnArguments(string arg1, int arg2, int? arg3, List<string> arg4, ArgObject arg5)
        {
            return "empty";
        }

        /// <summary>
        /// Overloaded.
        /// </summary>
        public void Overloaded() { }

        /// <summary>
        /// Overloaded.
        /// </summary>
        /// <param name="arg">The argument.</param>
        public void Overloaded(int arg) { }

        /// <summary>
        /// Private method.
        /// </summary>
        private void PrivateMethod() { }

        /// <summary>
        /// Protected method.
        /// </summary>
        protected void ProtectedMethod() { }

        /// <summary>
        /// Generic method.
        /// </summary>
        /// <typeparam name="TSomething">The type of something.</typeparam>
        public void GenericMethod<TSomething>() { }

        /// <summary>
        /// Arg object.
        /// </summary>
        public class ArgObject
        {
            /// <summary>
            /// Gets or sets the property1.
            /// </summary>
            /// <value>
            /// The property1.
            /// </value>
            public string Property1 { get; set; }
        }
    }
}
