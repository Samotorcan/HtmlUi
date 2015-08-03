using Microsoft.VisualStudio.TestTools.UnitTesting;
using Samotorcan.Tests.Core;

namespace Samotorcan.Tests.Windows
{
    /// <summary>
    /// Sync to client unit test.
    /// </summary>
    [TestClass]
    public class SyncToClientUnitTest : BaseUnitTest
    {
        #region Tests

        #region ControllerConstructorTest
        /// <summary>
        /// Controller constructor test.
        /// </summary>
        [TestMethod]
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
