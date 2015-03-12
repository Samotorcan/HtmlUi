using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Samotorcan.HtmlUi.Core.Browser;
using Samotorcan.HtmlUi.Core.Events;
using Samotorcan.HtmlUi.Core.Exceptions;
using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Samotorcan.HtmlUi.Core
{
    /// <summary>
    /// Base window.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class BaseWindow : IDisposable
    {
        #region Events

        #region BrowserCreated
        /// <summary>
        /// Occurs when browser is created.
        /// </summary>
        protected event EventHandler<BrowserCreatedEventArgs> BrowserCreated;
        #endregion
        #region KeyPress
        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        protected event EventHandler<KeyPressEventArgs> KeyPress;
        #endregion

        #endregion
        #region Properties
        #region Public

        #region View
        private string _view;
        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        /// <exception cref="System.InvalidOperationException">View can only be changed before the window is created.</exception>
        public string View
        {
            get
            {
                return _view;
            }
            set
            {
                BaseMainApplication.Current.EnsureMainThread();

                if (IsBrowserCreated)
                    throw new InvalidOperationException("View can only be changed before the window is created.");

                _view = value;
            }
        }
        #endregion
        #region Borderless
        private bool _borderless;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BaseWindow"/> is borderless.
        /// </summary>
        /// <value>
        ///   <c>true</c> if borderless; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Borderless
        {
            get
            {
                return _borderless;
            }
            set
            {
                BaseMainApplication.Current.EnsureMainThread();

                _borderless = value;
            }
        }
        #endregion

        #endregion
        #region Internal

        #region IsBrowserCreated
        /// <summary>
        /// Gets a value indicating whether browser is created.
        /// </summary>
        /// <value>
        /// <c>true</c> if browser is created; otherwise, <c>false</c>.
        /// </value>
        internal bool IsBrowserCreated { get; private set; }
        #endregion
        #region Controllers
        /// <summary>
        /// Gets or sets the controllers.
        /// </summary>
        /// <value>
        /// The controllers.
        /// </value>
        internal Dictionary<int, Controller> Controllers { get; set; }
        #endregion

        #endregion
        #region Protected

        #region CefBrowser
        /// <summary>
        /// Gets or sets the cef browser.
        /// </summary>
        /// <value>
        /// The cef browser.
        /// </value>
        protected CefBrowser CefBrowser { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWindow"/> class.
        /// </summary>
        protected BaseWindow()
        {
            View = "/Views/Index.html";
            Controllers = new Dictionary<int, Controller>();
        }

        #endregion
        #region Methods
        #region Internal

        #region TriggerKeyPress
        /// <summary>
        /// Triggers the key press.
        /// </summary>
        /// <param name="nativeKeyCode">The native key code.</param>
        internal void TriggerKeyPress(int nativeKeyCode)
        {
            if (KeyPress != null)
                KeyPress(this, new KeyPressEventArgs(nativeKeyCode));
        }
        #endregion
        #region CallFunction
        /// <summary>
        /// Calls the function.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="data">The data.</param>
        /// <exception cref="System.ArgumentNullException">name</exception>
        internal void CallFunction(string name, object data)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            MessageUtility.SendMessage(CefBrowser, "callFunction", null, new { Name = name, Data = data });
        }

        /// <summary>
        /// Calls the function.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentNullException">name</exception>
        internal void CallFunction(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            MessageUtility.SendMessage(CefBrowser, "callFunction", null, new { Name = name, Data = Undefined.Value });
        }
        #endregion

        #region CreateController
        /// <summary>
        /// Creates the controllers.
        /// </summary>
        internal Controller CreateController(string name, int id)
        {
            if (Controllers.ContainsKey(id))
                throw new ArgumentException("Controller with this id already exists.", "id");

            var controllerProvider = BaseMainApplication.Current.ControllerProvider;

            // create
            var controller = controllerProvider.CreateController(name, id);
            Controllers.Add(id, controller);

            // notify on property change    TODO: bundle changes
            controller.PropertyChanged += (sender, e) =>
            {
                var propertyName = e.PropertyName;
                var propertyValue = controller.GetPropertyValue(propertyName);

                // log
                GeneralLog.Debug(string.Format("Controller id: {0} property changed: {1} = {2}.",
                    controller.Id,
                    e.PropertyName,
                    JsonConvert.SerializeObject(propertyValue)));

                // sync controller changes
                CallFunction("syncControllerChanges", new List<ControllerChange>
                {
                    new ControllerChange
                    {
                        Id = controller.Id,
                        Properties = new Dictionary<string, JToken>
                        {
                            { e.PropertyName, JToken.FromObject(propertyValue) }
                        }
                    }
                });
            };

            return controller;
        }
        #endregion
        #region DestroyController
        /// <summary>
        /// Destroys the controller.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        internal bool DestroyController(int id)
        {
            if (Controllers.ContainsKey(id))
            {
                var controller = Controllers[id];

                controller.Dispose();
                Controllers.Remove(id);

                return true;
            }

            return false;
        }
        #endregion
        #region DestroyControllers
        /// <summary>
        /// Destroys the controllers.
        /// </summary>
        internal void DestroyControllers()
        {
            foreach (var controller in Controllers.Values)
                controller.Dispose();

            Controllers.Clear();
        }
        #endregion
        #region SyncControllerChanges
        /// <summary>
        /// Synchronizes the controller changes.
        /// </summary>
        /// <param name="controllerChanges">The controller changes.</param>
        /// <exception cref="ControllerNotFoundException"></exception>
        internal void SyncControllerChanges(IEnumerable<ControllerChange> controllerChanges)
        {
            BaseMainApplication.Current.EnsureMainThread();

            GeneralLog.Debug(string.Format("Sync controller changes: {0}", JsonConvert.SerializeObject(controllerChanges)));

            foreach (var controllerChange in controllerChanges)
            {
                if (!Controllers.ContainsKey(controllerChange.Id))
                    throw new ControllerNotFoundException();

                var controller = Controllers[controllerChange.Id];

                foreach (var changeProperty in controllerChange.Properties)
                    controller.SetPropertyValue(changeProperty.Key, changeProperty.Value);
            }
        }
        #endregion
        #region CallMethod
        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="controllerId">The controller identifier.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">methodName</exception>
        internal object CallMethod(int controllerId, string methodName, JArray arguments)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException("methodName");

            if (!Controllers.ContainsKey(controllerId))
                throw new ControllerNotFoundException();

            return Controllers[controllerId].CallMethod(methodName, arguments);
        }
        #endregion

        #endregion
        #region Protected

        #region CreateBrowser
        /// <summary>
        /// Creates the browser.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="position">The position.</param>
        protected void CreateBrowser(IntPtr handle, CefRectangle position)
        {
            BaseMainApplication.Current.EnsureMainThread();

            if (IsBrowserCreated)
                throw new InvalidOperationException("Browser already created.");

            var cefWindowInfo = CefWindowInfo.Create();
            cefWindowInfo.SetAsChild(handle, position);

            var cefClient = new Client();
            cefClient.BrowserCreated += (sender, e) =>
            {
                CefBrowser = e.CefBrowser;

                if (BrowserCreated != null)
                    BrowserCreated(this, e);
            };

            var cefSettings = new CefBrowserSettings();

            CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefSettings, BaseMainApplication.Current.GetAbsoluteContentUrl(View));

            IsBrowserCreated = true;
        }
        #endregion

        #endregion
        #endregion

        #region IDisposable

        /// <summary>
        /// Was dispose already called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            BaseMainApplication.Current.EnsureMainThread();

            if (!_disposed)
            {
                if (disposing)
                {
                    // CEF browser
                    if (CefBrowser != null)
                    {
                        var host = CefBrowser.GetHost();

                        if (host != null)
                        {
                            host.CloseBrowser(true);
                            host.Dispose();
                        }

                        CefBrowser.Dispose();
                    }

                    // controllers
                    foreach (var controller in Controllers.Values)
                        controller.Dispose();

                    Controllers.Clear();
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
