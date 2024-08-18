using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using NUnit.Framework;
using NLog;
using System;
using System.Collections;

namespace Webdriver.FinalTestTak
{
    public enum BrowserType
    {
        Chrome,
        Firefox,
        Edge
    }

    public class BrowserTypeSource : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return new TestFixtureData(BrowserType.Chrome);
            yield return new TestFixtureData(BrowserType.Firefox);
            yield return new TestFixtureData(BrowserType.Edge);
        }
    }

    [TestFixtureSource(typeof(BrowserTypeSource))]
    [Parallelizable(ParallelScope.All)]
    public class LoginPageTests
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private IWebDriver _driver;
        private LoginPage _loginPage;
        private readonly BrowserType _browserType;

        public LoginPageTests(BrowserType browserType)
        {
            _browserType = browserType;
        }

        [SetUp]
        public void SetUp()
        {
            Logger.Info("Starting the test setup.");
            _driver = CreateDriver(_browserType);
            _loginPage = new LoginPage(_driver);
            Logger.Info("Driver initialized and LoginPage instance created.");
        }

        [Test]
        public void LoginFormWithEmptyCredentials()
        {
            Logger.Info("Starting LoginFormWithEmptyCredentials test.");
            _loginPage.Open();
            Logger.Info("Opened login page.");

            _loginPage.FillOutTheForm();
            Logger.Info("Filled out the login form with empty credentials.");

            _loginPage.ClearTheForm();
            Logger.Info("Cleared the form.");

            _loginPage.SubmitTheForm();
            Logger.Info("Submitted the form.");

            _loginPage.CheckErrorMessage(LoginPage.ErrorMessageUsernameIsRequiered);
            Logger.Info("Checked error message for empty credentials.");
        }

        [Test]
        public void LoginFormWithCredentialsByPassingUsername()
        {
            Logger.Info("Starting LoginFormWithCredentialsByPassingUsername test.");
            _loginPage.Open();
            Logger.Info("Opened login page.");

            _loginPage.FillOutTheForm(LoginPage.AcceptedUserName, LoginPage.AcceptedPassword);
            Logger.Info("Filled out the form with username but without password.");

            _loginPage.ClearThePasswordField();
            Logger.Info("Cleared the password field.");

            _loginPage.SubmitTheForm();
            Logger.Info("Submitted the form.");

            _loginPage.CheckErrorMessage(LoginPage.ErrorMessagePasswordIsRequiered);
            Logger.Info("Checked error message for missing password.");
        }

        [Test]
        public void LoginFormWithCredentialsByPassingUsernameAndPassword()
        {
            Logger.Info("Starting LoginFormWithCredentialsByPassingUsernameAndPassword test.");
            _loginPage.Open();
            Logger.Info("Opened login page.");

            _loginPage.FillOutTheForm(LoginPage.AcceptedUserName, LoginPage.AcceptedPassword);
            Logger.Info("Filled out the form with valid username and password.");

            _loginPage.SubmitTheForm();
            Logger.Info("Submitted the form.");

            _loginPage.CheckLogin(LoginPage.SwagLabsPageUrl);
            Logger.Info("Checked login success and redirected to the main page.");
        }

        private IWebDriver CreateDriver(BrowserType browserType)
        {
            switch (browserType)
            {
                case BrowserType.Chrome:
                    return new ChromeDriver();
                case BrowserType.Firefox:
                    return new FirefoxDriver();
                case BrowserType.Edge:
                    return new EdgeDriver();
                default:
                    throw new NotSupportedException($"Browser type {browserType} is not supported");
            }
        }

        [TearDown]
        public void TearDown()
        {
            Logger.Info("Starting the teardown.");
            if (_driver != null)
            {
                _driver.Quit();
                _driver.Dispose();
            }
            Logger.Info("Driver quit and disposed.");
        }
    }
}
