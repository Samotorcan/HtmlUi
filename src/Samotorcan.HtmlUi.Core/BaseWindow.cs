using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Samotorcan.HtmlUi.Core.Browser;
using Samotorcan.HtmlUi.Core.Diagnostics;
using Samotorcan.HtmlUi.Core.Events;
using Samotorcan.HtmlUi.Core.Exceptions;
using Samotorcan.HtmlUi.Core.Logs;
using Samotorcan.HtmlUi.Core.Messages;
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

            MessageUtility.SendMessage(CefBrowser, "callFunction", null, new CallFunction { Name = name, Data = data });
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

            MessageUtility.SendMessage(CefBrowser, "callFunction", null, new CallFunction { Name = name, Data = Undefined.Value });
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
        #region SyncControllerChangesToServer
        /// <summary>
        /// Synchronizes the controller changes to server.
        /// </summary>
        /// <param name="controllerChanges">The controller changes.</param>
        /// <exception cref="ControllerNotFoundException"></exception>
        internal void SyncControllerChangesToServer(IEnumerable<ControllerChange> controllerChanges)
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
        #region SyncControllerChangesToClient
        /// <summary>
        /// Synchronizes the controller changes to client.
        /// </summary>
        internal void SyncControllerChangesToClient()
        {
            var application = BaseMainApplication.Current;

            application.EnsureMainThread();

            GeneralLog.Debug("Sync controller changes to client.");

            var controllerChanges = GetControllerChanges();

            if (controllerChanges.Any())
                CallFunction("syncControllerChanges", controllerChanges);
        }
        #endregion
        #region CallMethod
        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="controllerId">The controller identifier.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="internalMethod">if set to <c>true</c> [internal method].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">methodName</exception>
        /// <exception cref="ControllerNotFoundException"></exception>
        internal object CallMethod(int controllerId, string methodName, JArray arguments, bool internalMethod)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException("methodName");

            if (!Controllers.ContainsKey(controllerId))
                throw new ControllerNotFoundException();

            return Controllers[controllerId].CallMethod(methodName, arguments, internalMethod);
        }

        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="controllerId">The controller identifier.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">methodName</exception>
        /// <exception cref="ControllerNotFoundException"></exception>
        internal object CallMethod(int controllerId, string methodName, JArray arguments)
        {
            return CallMethod(controllerId, methodName, arguments, false);
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
        #region Private

        #region GetControllerChanges
        /// <summary>
        /// Gets the controller changes.
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, ControllerChange> GetControllerChanges()
        {
            return GetControllerChanges(1);
        }

        /// <summary>
        /// Gets the controller changes.
        /// </summary>
        /// <param name="depth">The depth.</param>
        /// <returns></returns>
        /// <exception cref="Samotorcan.HtmlUi.Core.Exceptions.SyncMaxDepthException"></exception>
        private Dictionary<int, ControllerChange> GetControllerChanges(int depth)
        {
            if (depth > BaseMainApplication.Current.SyncMaxDepth)
                throw new SyncMaxDepthException(BaseMainApplication.Current.SyncMaxDepth);

            var controllerChanges = new Dictionary<int, ControllerChange>();

            // controller changes
            foreach (var controller in Controllers.Values)
            {
                // property changes
                if (controller.PropertyChanges.Any())
                {
                    var propertyChanges = new HashSet<string>(controller.PropertyChanges);
                    controller.PropertyChanges = new HashSet<string>();

                    controllerChanges.Add(controller.Id, new ControllerChange
                    {
                        Id = controller.Id,
                        Properties = propertyChanges
                            .ToDictionary(p => p, p => JToken.FromObject(controller.GetPropertyValue(p)))
                    });
                }

                // observable collection changes
                if (controller.ObservableCollectionChanges.Any())
                {
                    if (!controllerChanges.ContainsKey(controller.Id))
                        controllerChanges.Add(controller.Id, new ControllerChange { Id = controller.Id });

                    var controllerChange = controllerChanges[controller.Id];

                    controllerChange.ObservableCollections = new Dictionary<string, ObservableCollectionChanges>(controller.ObservableCollectionChanges);
                    controller.ObservableCollectionChanges = new Dictionary<string, ObservableCollectionChanges>();

                    // change reset to property change
                    foreach (var observableCollection in controllerChange.ObservableCollections.Values)
                    {
                        if (observableCollection.IsReset)
                            controllerChange.Properties.Add(observableCollection.Name, JToken.FromObject(controller.GetPropertyValue(observableCollection.Name)));
                    }
                }

                // remove observable collection changes if not needed
                if (controllerChanges.ContainsKey(controller.Id))
                {
                    var controllerChange = controllerChanges[controller.Id];

                    var removeObservableCollectionKeys = controllerChange.ObservableCollections
                        .Where(o => controllerChange.Properties.ContainsKey(o.Value.Name) || o.Value.IsReset)
                        .Select(o => o.Key)
                        .ToList();

                    foreach (var removeObservableCollectionKey in removeObservableCollectionKeys)
                        controllerChange.ObservableCollections.Remove(removeObservableCollectionKey);
                }
            }

            // next changes triggered by getter for property
            if (controllerChanges.Any(c => c.Value.Properties.Any()))
            {
                // add next controller changes
                foreach (var nextControllerChange in GetControllerChanges(++depth).Values)
                {
                    if (!controllerChanges.ContainsKey(nextControllerChange.Id))
                        controllerChanges.Add(nextControllerChange.Id, new ControllerChange { Id = nextControllerChange.Id });

                    var controllerChange = controllerChanges[nextControllerChange.Id];

                    // properties
                    foreach (var nextProperty in nextControllerChange.Properties.Keys)
                    {
                        if (!controllerChange.Properties.ContainsKey(nextProperty))
                            controllerChange.Properties.Add(nextProperty, null);

                        controllerChange.Properties[nextProperty] = nextControllerChange.Properties[nextProperty];
                    }
                }
            }

            return controllerChanges;
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
