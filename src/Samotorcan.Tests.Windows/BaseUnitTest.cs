using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using Samotorcan.Tests.Core;
using System;

namespace Samotorcan.Tests.Windows
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

        #endregion
        #endregion
        #region Methods
        #region Public

        #region Initialize
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize]
        public virtual void Initialize()
        {
            DriverService = TestUtility.CreateDriverService();
            Driver = TestUtility.CreateDriver(DriverService);

            Driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(1));
        }
        #endregion
        #region Cleanup
        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        [TestCleanup]
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
