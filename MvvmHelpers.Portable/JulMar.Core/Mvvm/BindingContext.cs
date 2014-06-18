using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;

using JulMar.Interfaces;

namespace JulMar.Mvvm
{
	/// <summary>
	/// Class which allows for data binding in environments which do not support the 
	/// concept such as Xamarin.iOS, Xamarin.Android.
	/// </summary>
	public class BindingContext
	{
		/// <summary>
		/// Reports any errors in the bindings.
		/// </summary>
		public static event Action<string> Error;

		/// <summary>
		/// Events we look for to manage two-way binding.
		/// </summary>
		internal static string[] ValidEvents = { "EditingDidEnd", "ValueChanged", "Ended" };

        /// <summary>
        /// This creates the binding for a given model object.
        /// </summary>
        /// <typeparam name="T">ViewModel/Model source object type</typeparam>
        /// <param name="source">Source object</param>
        /// <returns>Created binding context</returns>
		public static BindingContext<T> Create<T>(T source)
			where T : class
		{
			return new BindingContext<T>(source);
		}

        /// <summary>
        /// Used to report an error in the binding. By default it outputs
        /// to the diagnostic trace.
        /// </summary>
        /// <param name="message">Error.</param>
		internal static void RaiseError(string message)
		{
			var error = Error;
			if (error != null)
				error(message);
			else
			{
				Debug.WriteLine("BindingContext: " + message);
			}
		}

        /// <summary>
        /// Constructor
        /// </summary>
        protected BindingContext()
        {
        }
	}

    /// <summary>
    /// Binding context returned when bindings are created.
    /// </summary>
    /// <typeparam name="TS">Source object type</typeparam>
	public class BindingContext<TS> : BindingContext, IDisposable
		where TS : class
	{
        TS source;

        /// <summary>
        /// Bindings currently managed by this context.
        /// </summary>
		readonly List<BindingItem> _targets = new List<BindingItem>();

        /// <summary>
        /// Binding source - if it implements INotifyPropertyChanged, then we hook into that.
        /// </summary>
		public TS Source {
			get {
				return source;
			}
			set {
				if (source != value) {
					INotifyPropertyChanged inpc;
					if (source != null)
					{
						inpc = source as INotifyPropertyChanged;
						if (inpc != null) {
							inpc.PropertyChanged -= OnSourceChanged;
						}
					}

					source = value;
					inpc = source as INotifyPropertyChanged;
					if (inpc != null) {
						inpc.PropertyChanged += OnSourceChanged;
					}
				}
			}
		}

        /// <summary>
        /// Internal constructor used to create the bindings.
        /// </summary>
        /// <param name="source"></param>
		internal BindingContext(TS source)
		{
			Source = source;
		}

        /// <summary>
        /// Class used to manage an individual binding.
        /// </summary>
		private abstract class BindingItem : IDisposable
		{
            /// <summary>
            /// Source object we are bound to.
            /// </summary>
            internal TS Source { get; set; }

            /// <summary>
            /// Converter to use.
            /// </summary>
			internal IValueConverter Converter { get; set; }

            /// <summary>
            /// Parameter passed to converter.
            /// </summary>
			internal object ConverterParameter { get; set; }

            /// <summary>
            /// Cleanup method.
            /// </summary>
			public virtual void Dispose()
			{
			}

            /// <summary>
            /// Used to update the target from the source
            /// </summary>
            internal abstract void UpdateTarget();

            /// <summary>
            /// Called when a property changes on our binding source.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
			public virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
			{
			}

		}

        /// <summary>
        /// This allows an action to be performed when an object changes.
        /// </summary>
        private class BindingItemAction<TSR> : BindingItem
        {
            /// <summary>
            /// Source property
            /// </summary>
            internal Expression<Func<TS, TSR>> SourceProperty { get; set; }

            /// <summary>
            /// Action to perform
            /// </summary>
            internal Action<TS> SourceChanged { get; set; }

            /// <summary>
            /// Retrieves the PropertyInfo descriptor for the source property.
            /// </summary>
            private PropertyInfo SourcePropertyInfo
            {
                get
                {
                    return (PropertyInfo)((MemberExpression)SourceProperty.Body).Member;
                }
            }

            /// <summary>
            /// Called when a property changes on our binding source.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (string.IsNullOrEmpty(e.PropertyName)
                    || SourcePropertyInfo.Name == e.PropertyName)
                {
                    UpdateTarget();
                }
            }

            /// <summary>
            /// Used to update the target from the source
            /// </summary>
            internal override void UpdateTarget()
            {
                try
                {
                    SourceChanged(Source);
                }
                catch (Exception ex)
                {
                    RaiseError(ex.Message);
                }
            }

        }

        /// <summary>
        /// Generic BindingItem
        /// </summary>
        /// <typeparam name="T">Target</typeparam>
        /// <typeparam name="TR">Target property type</typeparam>
        /// <typeparam name="TSR">Source property type</typeparam>
		private class BindingItem<T,TR,TSR> : BindingItem
		{
            private bool updating;

            /// <summary>
            /// Source property
            /// </summary>
            internal Expression<Func<TS, TSR>> SourceProperty { get; set; }
            
            /// <summary>
            /// Target object
            /// </summary>
            internal T Target { get; set; }
            /// <summary>
            /// Target property
            /// </summary>
            internal Expression<Func<T, TR>> TargetProperty { get; set; }

            /// <summary>
            /// Cleanup method.
            /// </summary>
            public override void Dispose()
			{
				Unsubscribe(Target, TargetPropertyInfo.Name + "Changed");
				foreach (var pn in ValidEvents)
					Unsubscribe(Target, pn);
			}

            /// <summary>
            /// Called when a property changes on our binding source.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				if (string.IsNullOrEmpty(e.PropertyName)
					|| SourcePropertyInfo.Name == e.PropertyName)
				{
					UpdateTarget();
				}
			}

            /// <summary>
            /// Retrieves the PropertyInfo descriptor for the source property.
            /// </summary>
            private PropertyInfo SourcePropertyInfo
			{
				get 
				{
					return (PropertyInfo)((MemberExpression)SourceProperty.Body).Member;
				}
			}

            /// <summary>
            /// Retrieves the PropertyInfo descriptor for the target property.
            /// </summary>
            private PropertyInfo TargetPropertyInfo
			{
				get 
				{
					return (PropertyInfo)((MemberExpression)TargetProperty.Body).Member;
				}
			}

            /// <summary>
            /// Used to update the target from the source
            /// </summary>
            internal override void UpdateTarget()
			{
				if (updating)
					return;

				updating = true;
                try
                {
                    var tp = TargetPropertyInfo;
                    var sp = SourcePropertyInfo;

                    object value = sp.GetValue(Source);
                    if (Converter != null)
                        value = Converter.Convert(value, tp.PropertyType, ConverterParameter, null);

                    TransferValue(value, Target, tp);
                }
                finally
                {
                    updating = false;
                }
			}

            /// <summary>
            /// Used to update the source from the target
            /// </summary>
            internal void UpdateSource()
			{
  				if (updating)
					return;

				updating = true;
                try
                {
                    var tp = TargetPropertyInfo;
                    var sp = SourcePropertyInfo;

                    object value = tp.GetValue(Target);
                    if (Converter != null)
                        value = Converter.ConvertBack(value, tp.PropertyType, ConverterParameter, null);

                    TransferValue(value, Source, sp);
                }
                finally
                {
                    updating = false;
                }
			}

            /// <summary>
            /// Creates a two-way binding where the information flows from target -> source.
            /// </summary>
			public void BindTwoWay()
			{
				if (!Subscribe(Target, TargetPropertyInfo.Name + "Changed"))
				{
					foreach (var pn in ValidEvents)
					{
						Subscribe(Target, pn);
					}
				}
			}

			/// <summary>
			/// Wires up an event to the target
			/// </summary>
			/// <param name="target"></param>
			/// <param name = "eventName"></param>
			internal bool Subscribe(object target, string eventName)
			{
				if (target != null)
				{
				    EventInfo ei = target.GetType().GetRuntimeEvents().FirstOrDefault(e => e.Name == eventName);
					if (ei != null)
					{
						ei.RemoveEventHandler(target, GetEventMethod(ei));
						ei.AddEventHandler(target, GetEventMethod(ei));
						return true;
					}
				}
				return false;
			}

			/// <summary>
			/// Unwires target event
			/// </summary>
			/// <param name="target"></param>
			/// <param name = "eventName"></param>
			internal void Unsubscribe(object target, string eventName)
			{
				if (target != null)
				{
                    EventInfo ei = target.GetType().GetRuntimeEvents().FirstOrDefault(e => e.Name == eventName);
					if (ei != null)
						ei.RemoveEventHandler(target, GetEventMethod(ei));
				}
			}

			private Delegate _method;
			private Delegate GetEventMethod(EventInfo ei)
			{
				if (ei == null)
					throw new ArgumentNullException("ei");
				if (ei.EventHandlerType == null)
					throw new ArgumentException("EventHandlerType is null");

                if (_method == null)
                {
                    MethodInfo eventHandler = GetType().GetRuntimeMethods().First(mi => mi.Name == "_OnEventRaised");
                    _method = eventHandler.CreateDelegate(ei.EventHandlerType, this);
                }

			    return _method;
			}

			/// <summary>
			/// This is invoked by the event - it invokes the command.
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			public void _OnEventRaised(object sender, EventArgs e)
			{
				UpdateSource();
			}

            /// <summary>
            /// Transfers a value from one property to another.
            /// </summary>
            /// <param name="newValue">Value to transfer</param>
            /// <param name="target">Target object</param>
            /// <param name="pi">Property to set</param>
			void TransferValue(object newValue, object target, PropertyInfo pi)
			{
				if (pi.CanWrite)
				{
					try {
						if (pi.PropertyType.GetTypeInfo().IsAssignableFrom(newValue.GetType().GetTypeInfo()))
						{
							pi.SetValue(target, newValue);
						}
						else
						{
							object value;
							if (this.AttemptTypeConversion(newValue.GetType(), pi.PropertyType, newValue, out value))
							{
								pi.SetValue(target, value);
							}
							else
							{
								RaiseError(string.Format("Failed to convert {0} to a {1}, Source={2}.{3}, Target={4}.{5}",
									newValue, pi.PropertyType, Source, SourceProperty, 
									Target, TargetProperty));
							}
						}
					}
					catch (Exception ex)
					{
						RaiseError(ex.Message);
					}
				}
				else
				{
					RaiseError(string.Format("{0}.{1} is read-only and cannot be set.", target.GetType(), pi.Name));
				}
			}

            /// <summary>
            /// This attempts simple type conversion, primarily for numeric types to/from strings.
            /// </summary>
            /// <param name="source">Source type</param>
            /// <param name="dest">Destination type</param>
            /// <param name="value">Value to convert</param>
            /// <param name="newValue">Returning value</param>
            /// <returns>True/False success</returns>
			bool AttemptTypeConversion(Type source, Type dest, object value, out object newValue)
            {
                if (value == null)
                {
                    newValue = dest.GetTypeInfo().IsValueType 
                        ? Activator.CreateInstance(dest) : null;
                    return true;
                }

                if (dest == typeof(string))
                {
                    newValue = value.ToString();
                    return true;
                }

                newValue = null;
                string sValue = value.ToString();

                if (dest == typeof(byte))
                {
                    byte result;
                    if (byte.TryParse(sValue, out result))
                    {
                        newValue = result;
                        return true;
                    }
                    return false;
                }

                if (dest == typeof(char))
                {
                    char result;
                    if (char.TryParse(sValue, out result))
                    {
                        newValue = result;
                        return true;
                    }
                    return false;
                }

                if (dest == typeof(short))
                {
                    short result;
                    if (short.TryParse(sValue, out result))
                    {
                        newValue = result;
                        return true;
                    }
                    return false;
                }

                if (dest == typeof(ushort))
                {
                    ushort result;
                    if (ushort.TryParse(sValue, out result))
                    {
                        newValue = result;
                        return true;
                    }
                    return false;
                }

                if (dest == typeof(int))
                {
                    int result;
                    if (int.TryParse(sValue, out result))
                    {
                        newValue = result;
                        return true;
                    }
                    return false;
                }

                if (dest == typeof(uint))
                {
                    uint result;
                    if (uint.TryParse(sValue, out result))
                    {
                        newValue = result;
                        return true;
                    }
                    return false;
                }

                if (dest == typeof(long))
                {
                    long result;
                    if (long.TryParse(sValue, out result))
                    {
                        newValue = result;
                        return true;
                    }
                    return false;
                }

                if (dest == typeof(ulong))
                {
                    ulong result;
                    if (ulong.TryParse(sValue, out result))
                    {
                        newValue = result;
                        return true;
                    }
                    return false;
                }

                if (dest == typeof(double))
                {
                    double result;
                    if (double.TryParse(sValue, out result))
                    {
                        newValue = result;
                        return true;
                    }
                    return false;
                }

                if (dest == typeof(decimal))
                {
                    decimal result;
                    if (decimal.TryParse(sValue, out result))
                    {
                        newValue = result;
                        return true;
                    }
                    return false;
                }

                return false;
            }
		}

        /// <summary>
        /// Method to create a binding
        /// </summary>
        /// <typeparam name="T">Target object Type</typeparam>
        /// <typeparam name="TR">Target return property type</typeparam>
        /// <typeparam name="TSR">Source property type</typeparam>
        /// <param name="sourceProperty">Source Property</param>
        /// <param name="target">Target Object</param>
        /// <param name="targetProperty">Target Property</param>
        /// <param name="twoWay">Binds two way</param>
        /// <param name="valueConverter">Value converter to use</param>
        /// <param name="converterParameter">Converter parameter</param>
        /// <returns>Binding Context for fluid use</returns>
        public BindingContext<TS> Add<T,TR, TSR>(
			Expression<Func<TS,TSR>> sourceProperty,
			T target, 
			Expression<Func<T,TR>> targetProperty, 
			bool twoWay = false, 
            IValueConverter valueConverter = null, object converterParameter = null) 
            where T : class
		{
            if (sourceProperty == null)
            {
                throw new ArgumentNullException("sourceProperty");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (targetProperty == null)
            {
                throw new ArgumentNullException("targetProperty");
            }
            if (sourceProperty.NodeType != ExpressionType.Lambda
				|| targetProperty.NodeType != ExpressionType.Lambda)
			{
				throw new Exception("Delegates must point to properties for source and target");
			}

			var bindingItem = new BindingItem<T,TR,TSR>() {
				Source = this.Source,
				SourceProperty = sourceProperty,
				Target = target,
				TargetProperty = targetProperty,
				Converter = valueConverter,
				ConverterParameter = converterParameter
			};

			if (twoWay)
				bindingItem.BindTwoWay();

			_targets.Add(bindingItem);
            bindingItem.UpdateTarget();

			return this;
		}

        /// <summary>
        /// This adds a binding which applies a target action each time the given
        /// source property is altered.
        /// </summary>
        /// <typeparam name="TSR"></typeparam>
        /// <param name="sourceProperty"></param>
        /// <param name="targetAction"></param>
        /// <returns></returns>
        public BindingContext<TS> Add<TSR>(Expression<Func<TS, TSR>> sourceProperty, Action<TS> targetAction)
        {
            if (sourceProperty == null)
            {
                throw new ArgumentNullException("sourceProperty");
            }
            if (targetAction == null)
            {
                throw new ArgumentNullException("targetAction");
            }

            if (sourceProperty.NodeType != ExpressionType.Lambda)
            {
                throw new Exception("Delegates must point to properties.");
            }

            var binding = new BindingItemAction<TSR>
                {
                    Source = this.Source,
                    SourceProperty = sourceProperty,
                    SourceChanged = targetAction
                };
            _targets.Add(binding);
            binding.UpdateTarget();
            
            return this;
        }

        /// <summary>
        /// Called when any property on the source object changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void OnSourceChanged(object sender, PropertyChangedEventArgs e)
		{
			foreach (var entry in _targets)
			{
				entry.OnPropertyChanged(sender, e);
			}
		}

        /// <summary>
        /// Called to cleanup the binding context.
        /// </summary>
		public void Dispose()
		{
			foreach (var item in _targets)
			{
				item.Dispose();
			}
			_targets.Clear();
		}
	}
}

