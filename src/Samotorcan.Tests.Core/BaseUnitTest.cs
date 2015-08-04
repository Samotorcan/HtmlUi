using OpenQA.Selenium.Chrome;
using System;
using NUnit.Framework;

namespace Samotorcan.Tests.Core
{
    /// <summary>
    /// Base unit test.
    /// </summary>
    public abstract class BaseUnitTest
    {
        #region Properties
        #region Protected

        #region DriverService
        /// <summary>
        /// Gets or sets the driver service.
        /// </summary>
        /// <value>
        /// The driver service.
        /// </value>
        protected ChromeDriverService DriverService { get; set; }
        #endregion
        #region Driver
        /// <summary>
        /// Gets or sets the driver.
        /// </summary>
        /// <value>
        /// The driver.
        /// </value>
        protected ChromeDriver Driver { get; set; }
        #endregion
        #region ApplicationName
        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        protected abstract string ApplicationName { get; }
        #endregion

        #endregion
        #endregion
        #region Methods
        #region Public

        #region Initialize
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestFixtureSetUp]
        public virtual void Initialize()
        {
            DriverService = TestUtility.CreateDriverService();
            Driver = TestUtility.CreateDriver(DriverService, ApplicationName);

            Driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(1));
        }
        #endregion
        #region Cleanup
        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        [TestFixtureTearDown]
        public virtual void Cleanup()
        {
            Driver.Quit();
            DriverService.Dispose();
        }
        #endregion

        #endregion
        #endregion
    }
}
