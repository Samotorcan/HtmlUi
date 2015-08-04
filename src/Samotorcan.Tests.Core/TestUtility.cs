using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Reflection;

namespace Samotorcan.Tests.Core
{
    /// <summary>
    /// Test utility.
    /// </summary>
    public static class TestUtility
    {
        #region Properties
        #region Public

        #region AssemblyDirectory
        /// <summary>
        /// Gets the assembly directory.
        /// </summary>
        /// <value>
        /// The assembly directory.
        /// </value>
        public static string AssemblyDirectory
        {
            get
            {
                return Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath).Replace('\\', '/');
            }
        }
        #endregion

        #endregion
        #endregion
        #region Methods
        #region Public

        #region CreateDriverService
        /// <summary>
        /// Creates the driver service.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "It's returned.")]
        public static ChromeDriverService CreateDriverService()
        {
            var driverService = ChromeDriverService.CreateDefaultService(AssemblyDirectory, "chromedriver.exe");

            driverService.Port = 21480;
            driverService.LogPath = AssemblyDirectory + "/chromedriver.log";
            driverService.EnableVerboseLogging = true;
            driverService.HideCommandPromptWindow = true;

            driverService.Start();

            return driverService;
        }
        #endregion
        #region CreateDriver
        /// <summary>
        /// Creates the driver.
        /// </summary>
        /// <param name="driverService">The driver service.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns></returns>
        public static ChromeDriver CreateDriver(ChromeDriverService driverService, string applicationName)
        {
            return new ChromeDriver(driverService, new ChromeOptions
            {
                BinaryLocation = AssemblyDirectory + "/" + applicationName
            });
        }
        #endregion
        #region FactoryHasFunction
        /// <summary>
        /// Factorie has function.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="function">The function.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "ChromeDriver is better.")]
        public static bool FactoryHasFunction(ChromeDriver driver, string factory, string function)
        {
            if (driver == null)
                throw new ArgumentNullException("driver");

            return (bool)driver.ExecuteScript(string.Format(@"
                try {{
                    var factory = angular.element(document.body).injector().get('{0}');

                    return angular.isFunction(factory['{1}']);
                }} catch (err) {{
                    return false;
                }}
            ", factory, function));
        }
        #endregion

        #endregion
        #endregion
    }
}
