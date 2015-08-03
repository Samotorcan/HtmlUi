using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Samotorcan.HtmlUi.Core.Utilities
{
    /// <summary>
    /// Expression utility.
    /// </summary>
    internal static class ExpressionUtility
    {
        #region Methods
        #region Public

        #region CreateMethodDelegate
        /// <summary>
        /// Creates the method delegate.
        /// </summary>
        /// <param name="methodInfo">The method information.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">methodInfo</exception>
        public static Delegate CreateMethodDelegate(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException("methodInfo");

            var actionTypeArgs = new List<Type> { methodInfo.ReflectedType };
            actionTypeArgs.AddRange(methodInfo.GetParameters()
                .Select(p => p.ParameterType)
                .ToList());

            var instanceParameter = Expression.Parameter(methodInfo.ReflectedType, "o");

            var methodArgumentParameters = methodInfo.GetParameters()
                .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                .ToList();

            var lambdaParameters = new List<ParameterExpression> { instanceParameter };
            lambdaParameters.AddRange(methodArgumentParameters);

            Type actionType = null;
            if (methodInfo.ReturnType == typeof(void))
            {
                actionType = Expression.GetActionType(actionTypeArgs.ToArray());
            }
            else
            {
                actionTypeArgs.Add(methodInfo.ReturnType);
                actionType = Expression.GetFuncType(actionTypeArgs.ToArray());
            }

            return Expression.Lambda(actionType, Expression.Call(instanceParameter, methodInfo, methodArgumentParameters), lambdaParameters)
                .Compile();
        }
        #endregion

        #endregion
        #endregion
    }
}
