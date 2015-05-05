using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Reflection;
using System.IO;
using System.Threading;

namespace Samotorcan.Tests.Windows
{
    [TestClass]
    public class UnitTest1
    {
        private ChromeDriverService DriverService { get; set; }
        private ChromeDriver Driver { get; set; }
        private string WorkingDirectory { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            WorkingDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath).Replace('\\', '/');

            DriverService = ChromeDriverService.CreateDefaultService(WorkingDirectory, "chromedriver.exe");
            DriverService.Start();

            Driver = new ChromeDriver(DriverService, new ChromeOptions
            {
                BinaryLocation = WorkingDirectory + "/Samotorcan.Tests.Windows.Application.exe",
                DebuggerAddress = "localhost:20480"
            });
        }

        [TestMethod]
        public void TestMethod1()
        {
            Driver.Url = "http://localhost/Views/Index.html";
            Thread.Sleep(60 * 1000);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Driver.Quit();
            DriverService.Dispose();
        }
    }
}
