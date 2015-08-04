using System;
using NUnit.Framework;
using Samotorcan.Tests.Core;

namespace Samotorcan.Tests
{
    /// <summary>
    /// All tests.
    /// </summary>
    [TestFixture]
    public class AllTests : BaseUnitTest
    {
        #region Properties
        #region Protected

        #region ApplicationName
        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        protected override string ApplicationName
        {
            get
            {
                return "Samotorcan.Tests.Application.exe";
            }
        }
        #endregion

        #endregion
        #endregion
        #region Tests

        #region ControllerConstructorTest
        /// <summary>
        /// Controller constructor test.
        /// </summary>
        [Test]
        public void ControllerConstructorTest()
        {
            Driver.Url = "http://localhost/Views/ControllerConstructorTest.html";

            Assert.IsTrue(TestUtility.FactoryHasFunction(Driver, "ControllerConstructorTest", "voidNoArguments"));
            Assert.IsTrue(TestUtility.FactoryHasFunction(Driver, "ControllerConstructorTest", "voidArguments"));
            Assert.IsTrue(TestUtility.FactoryHasFunction(Driver, "ControllerConstructorTest", "returnNoArguments"));
            Assert.IsTrue(TestUtility.FactoryHasFunction(Driver, "ControllerConstructorTest", "returnArguments"));

            Assert.IsFalse(TestUtility.FactoryHasFunction(Driver, "ControllerConstructorTest", "privateMethod"));
            Assert.IsFalse(TestUtility.FactoryHasFunction(Driver, "ControllerConstructorTest", "protectedMethod"));
            Assert.IsFalse(TestUtility.FactoryHasFunction(Driver, "ControllerConstructorTest", "overloaded"));
            Assert.IsFalse(TestUtility.FactoryHasFunction(Driver, "ControllerConstructorTest", "genericMethod"));
        }
        #endregion

        #endregion
    }
}
