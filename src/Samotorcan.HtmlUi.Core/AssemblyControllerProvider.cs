using Samotorcan.HtmlUi.Core.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core
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

        #endregion
        #region Private

        #region ControllerTypes
        /// <summary>
        /// Gets or sets the controller types.
        /// </summary>
        /// <value>
        /// The controller types.
        /// </value>
        private List<Type> ControllerTypes { get; set; }
        #endregion

        #endregion
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyControllerProvider"/> class.
        /// </summary>
        public AssemblyControllerProvider()
        {
            ControllerTypes = new List<Type>();

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
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public Controller CreateController(string path)
        {
            Argument.Null(path, "path");

            var controllerType = GetControllerType(path);

            return (Controller)Activator.CreateInstance(controllerType);
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
        #region GetControllerType
        /// <summary>
        /// Gets the controller type.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// Found more than one controller with the same name.;path
        /// or
        /// Controller was not found.;path
        /// </exception>
        private Type GetControllerType(string path)
        {
            var controllerType = ControllerTypes.Where(c => c.FullName == path).SingleOrDefault();

            // search by partial match
            if (controllerType == null)
            {
                var partialMatchControllerTypes = ControllerTypes.Where(c => c.FullName.Split('.').Last() == path).ToList();

                if (partialMatchControllerTypes.Count > 1)
                    throw new ArgumentException("Found more than one controller with the same name.", "path");

                controllerType = partialMatchControllerTypes.FirstOrDefault();
            }

            if (controllerType == null) // TODO: go with null return
                throw new ArgumentException("Controller was not found.", "path");

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
            ControllerTypes.Clear();

            foreach (var type in Assemblies.SelectMany(a => a.GetTypes()))
            {
                if (IsControllerType(type))
                    ControllerTypes.Add(type);
            }
        }
        #endregion

        #endregion
        #endregion
    }
}
