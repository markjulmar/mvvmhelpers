using System;
using System.Linq;
using System.Reflection;
using System.Windows.Interactivity;
using Windows.UI.Xaml;

namespace JulMar.Windows.Interactivity.Actions
{
    public class CallMethodAction : TargetedTriggerAction<object>
    {
        /// <summary>
        /// Backing storage for the method name
        /// </summary>
        public static readonly DependencyProperty MethodNameProperty = DependencyProperty.Register("MethodName", typeof(string), typeof(CallMethodAction), null);

        /// <summary>
        /// Method name to invoke
        /// </summary>
        public string MethodName
        {
            get
            {
                return (string) base.GetValue(MethodNameProperty);
            }
            set
            {
                base.SetValue(MethodNameProperty, value);
            }
        }

        /// <summary>
        /// Check the method parameters for the invoke.
        /// </summary>
        /// <param name="methodParams"></param>
        /// <returns></returns>
        private static bool AreMethodParamsValid(ParameterInfo[] methodParams)
        {
            return methodParams.Length == 2
                       ? methodParams[0].ParameterType == typeof (object) &&
                         typeof (EventArgs).GetTypeInfo().IsAssignableFrom(methodParams[1].ParameterType.GetTypeInfo())
                       : methodParams.Length == 0;
        }

        /// <summary>
        /// Invoke method - must be overridden
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {
            if (base.AssociatedObject != null)
            {
                MethodInfo descriptor = this.FindBestMethod(parameter);
                if (descriptor != null)
                {
                    ParameterInfo[] parameters = descriptor.GetParameters();
                    if (parameters.Length == 0)
                    {
                        descriptor.Invoke(this.Target, null);
                    }
                    else if (parameters.Length == 2 
                        && AssociatedObject != null
                        && parameter != null && parameters[0].ParameterType.GetTypeInfo().IsAssignableFrom(AssociatedObject.GetType().GetTypeInfo())
                        && parameters[1].ParameterType.GetTypeInfo().IsAssignableFrom(parameter.GetType().GetTypeInfo()))
                    {
                        descriptor.Invoke(this.Target, new object[] { base.AssociatedObject, parameter });
                    }
                }
                else if (this.TargetObject != null)
                {
                    throw new ArgumentException("No method found to invoked");
                }
            }
        }

        /// <summary>
        /// Locates the best method to invoke.  We first look for a 2-parameter method which takes the associated object type and parameter type.
        /// If we can't find that, then we look for a no parameter method.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private MethodInfo FindBestMethod(object parameter)
        {
            MethodInfo foundMethod = null;

            // Look for the two-parameter case
            if (parameter != null && AssociatedObject != null)
            {
                foundMethod = Target.GetType().GetTypeInfo().DeclaredMethods.FirstOrDefault(
                    mi => mi.Name == MethodName && mi.GetParameters().Length == 2
                    && mi.GetParameters()[0].ParameterType.GetTypeInfo().IsAssignableFrom(AssociatedObject.GetType().GetTypeInfo())
                    && mi.GetParameters()[1].ParameterType.GetTypeInfo().IsAssignableFrom(parameter.GetType().GetTypeInfo()));
            }

            return foundMethod ?? Target.GetType().GetTypeInfo().DeclaredMethods.FirstOrDefault(
                mi => mi.Name == MethodName && mi.GetParameters().Length == 0);
        }

        /// <summary>
        /// Check the method to see if it matches the name and has a void return type
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private bool IsMethodValid(MethodInfo method)
        {
            return string.Equals(method.Name, this.MethodName, StringComparison.Ordinal) &&
                   method.ReturnType == typeof(void);
        }
    }
}
