using Samotorcan.HtmlUi.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

namespace Samotorcan.HtmlUi.Core.Providers
{
    /// <summary>
    /// Assembly controller provider.
    /// </summary>
    public class AssemblyControllerProvider : IControllerProvider
    {
        #region Properties
        #region Public

        #region Assemblies
        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <value>
        /// The assemblies.
        /// </value>
        public ObservableCollection<Assembly> Assemblies { get; private set; }
        #endregion
        #region ControllerTypes
        /// <summary>
        /// Gets controller types.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> ControllerTypes
        {
            get
            {
                return ControllerTypesInternal.ToList();
            }
        }
        #endregion

        #endregion
        #region Private

        #region ControllerTypesInternal
        /// <summary>
        /// Gets or sets the controller types internal.
        /// </summary>
        /// <value>
        /// The controller types internal.
        /// </value>
        private List<Type> ControllerTypesInternal { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyControllerProvider"/> class.
        /// </summary>
        public AssemblyControllerProvider()
        {
            ControllerTypesInternal = new List<Type>();

            Assemblies = new ObservableCollection<Assembly>();
            Assemblies.CollectionChanged += Assemblies_CollectionChanged;

            // add entry assembly as default
            Assemblies.Add(Assembly.GetEntryAssembly());
        }

        #endregion
        #region Methods
        #region Public

        #region CreateController
        /// <summary>
        /// Creates the controller.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public Controller CreateController(string name, int id)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            var controller = (Controller)Activator.CreateInstance(GetControllerType(name));
            controller.Initialize(id);

            return controller;
        }
        #endregion
        #region CreateObservableController
        /// <summary>
        /// Creates the observable controller.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public ObservableController CreateObservableController(string name, int id)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            var observableController = (ObservableController)Activator.CreateInstance(GetObservableControllerType(name));
            observableController.Initialize(id);

            return observableController;
        }
        #endregion
        #region IsUniqueControllerName
        /// <summary>
        /// Determines whether the specified controller name is unique.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public bool IsUniqueControllerName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            var controllerTypesCount = ControllerTypesInternal.Where(c => c.Name == name).Count();

            if (controllerTypesCount == 0)
                throw new ControllerNotFoundException(name);

            return controllerTypesCount == 1;
        }
        #endregion
        #region ControllerExists
        /// <summary>
        /// Controller exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public bool ControllerExists(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            return ControllerTypesInternal.Where(c => c.Name == name).Count() > 0;
        }
        #endregion

        #endregion
        #region Private

        #region IsControllerType
        /// <summary>
        /// Determines whether type is controller type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private bool IsControllerType(Type type)
        {
            // can't create an abstract type
            if (type.IsAbstract)
                return false;

            // is extended from Controller
            while ((type = type.BaseType) != null)
            {
                if (type == typeof(Controller))
                    return true;
            }

            return false;
        }
        #endregion
        #region IsObservableControllerType
        /// <summary>
        /// Determines whether [is observable controller type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private bool IsObservableControllerType(Type type)
        {
            // can't create an abstract type
            if (type.IsAbstract)
                return false;

            // is extended from Controller
            while ((type = type.BaseType) != null)
            {
                if (type == typeof(ObservableController))
                    return true;
            }

            return false;
        }
        #endregion
        #region GetControllerType
        /// <summary>
        /// Gets the controller type.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// Found more than one controller with the same name.;path
        /// or
        /// Controller was not found.;path
        /// </exception>
        private Type GetControllerType(string name)
        {
            var controllerTypes = ControllerTypesInternal.Where(c => c.Name == name).ToList();

            if (controllerTypes.Count > 1)
                throw new MoreThanOneControllerFoundException(name);

            if (!controllerTypes.Any())
                throw new ControllerNotFoundException(name);

            return controllerTypes.FirstOrDefault();
        }
        #endregion
        #region GetObservableControllerType
        /// <summary>
        /// Gets the controller type.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// Found more than one controller with the same name.;path
        /// or
        /// Controller was not found.;path
        /// </exception>
        private Type GetObservableControllerType(string name)
        {
            var controllerTypes = ControllerTypesInternal.Where(c => c.Name == name).ToList();

            if (controllerTypes.Count > 1)
                throw new MoreThanOneControllerFoundException(name);

            if (!controllerTypes.Any())
                throw new ControllerNotFoundException(name);

            var controllerType = controllerTypes.First();

            if (!IsObservableControllerType(controllerType))
                throw new ControllerNotFoundException(name);

            return controllerType;
        }
        #endregion

        #region Assemblies_CollectionChanged
        /// <summary>
        /// Handles the CollectionChanged event of the Assemblies control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void Assemblies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // set controller types
            ControllerTypesInternal.Clear();

            foreach (var type in Assemblies.SelectMany(a => a.GetTypes()))
            {
                if (IsControllerType(type))
                    ControllerTypesInternal.Add(type);
            }
        }
        #endregion

        #endregion
        #endregion
    }
}
