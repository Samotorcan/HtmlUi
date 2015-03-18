using Samotorcan.HtmlUi.Core.Logs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.HtmlUi.Core.Diagnostics
{
    /// <summary>
    /// Stopwatch.
    /// </summary>
    internal static class Stopwatch
    {
        #region Methods
        #region Public

        #region Measure
        /// <summary>
        /// Measures the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentException">action</exception>
        public static void Measure(Action action, [CallerMemberName] string name = null)
        {
            if (action == null)
                throw new ArgumentException("action");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                action();
            }
            finally
            {
                stopwatch.Stop();

                Stopwatch.LogMeasure(stopwatch, name);
            }
        }

        /// <summary>
        /// Measures the specified action.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">action</exception>
        public static TReturn Measure<TReturn>(Func<TReturn> action, [CallerMemberName] string name = null)
        {
            if (action == null)
                throw new ArgumentException("action");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                return action();
            }
            finally
            {
                stopwatch.Stop();

                Stopwatch.LogMeasure(stopwatch, name);
            }
        }

        /// <summary>
        /// Measures the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentException">action</exception>
        public static void Measure(Action<System.Diagnostics.Stopwatch> action, [CallerMemberName] string name = null)
        {
            if (action == null)
                throw new ArgumentException("action");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                action(stopwatch);
            }
            finally
            {
                stopwatch.Stop();

                Stopwatch.LogMeasure(stopwatch, name);
            }
        }

        /// <summary>
        /// Measures the specified action.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">action</exception>
        public static TReturn Measure<TReturn>(Func<System.Diagnostics.Stopwatch, TReturn> action, [CallerMemberName] string name = null)
        {
            if (action == null)
                throw new ArgumentException("action");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                return action(stopwatch);
            }
            finally
            {
                stopwatch.Stop();

                Stopwatch.LogMeasure(stopwatch, name);
            }
        }
        #endregion

        #endregion
        #region Private

        #region LogMeasure
        /// <summary>
        /// Logs the measure.
        /// </summary>
        /// <param name="stopwatch">The stopwatch.</param>
        /// <param name="name">The name.</param>
        private static void LogMeasure(System.Diagnostics.Stopwatch stopwatch, string name)
        {
            GeneralLog.Debug(string.Format("[{0}ms] - {1}", stopwatch.Elapsed.TotalMilliseconds.ToString(CultureInfo.InvariantCulture), name));
        }
        #endregion

        #endregion
        #endregion
    }
}
