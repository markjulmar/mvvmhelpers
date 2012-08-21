using System;
using System.Reflection;
using System.Windows.Interactivity;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// Method used to change a property value when a trigger is fired.
    /// </summary>
    public class ChangePropertyAction : TargetedTriggerAction<object>
    {
        /// <summary>
        /// Backing storage for Duration property
        /// </summary>
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(Duration), typeof(ChangePropertyAction), null);

        /// <summary>
        /// Duration of the animation used
        /// </summary>
        public Duration Duration
        {
            get
            {
                return (Duration)base.GetValue(DurationProperty);
            }
            set
            {
                base.SetValue(DurationProperty, value);
            }
        }

        /// <summary>
        /// Backing storage for the PropertyName
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(ChangePropertyAction), null);

        /// <summary>
        /// Property to change
        /// </summary>
        public string PropertyName
        {
            get
            {
                return (string)base.GetValue(PropertyNameProperty);
            }
            set
            {
                base.SetValue(PropertyNameProperty, value);
            }
        }

        /// <summary>
        /// Backing storage for the new value
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(ChangePropertyAction), null);

        /// <summary>
        /// New value to set on property
        /// </summary>
        public object Value
        {
            get
            {
                return base.GetValue(ValueProperty);
            }
            set
            {
                base.SetValue(ValueProperty, value);
            }
        }

        /// <summary>
        /// Backing storage for the easing animation function
        /// </summary>
        public static readonly DependencyProperty EaseProperty = DependencyProperty.Register("Ease", typeof(EasingFunctionBase), typeof(ChangePropertyAction), null);

        /// <summary>
        /// Optional easing function for the animation (used if duration is set)
        /// </summary>
        public EasingFunctionBase Ease
        {
            get
            {
                return (EasingFunctionBase)base.GetValue(EaseProperty);
            }
            set
            {
                base.SetValue(EaseProperty, value);
            }
        }

        /// <summary>
        /// This method is used to change the property value using an animation.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="fromValue"></param>
        /// <param name="newValue"></param>
        private void AnimatePropertyChange(PropertyInfo propertyInfo, object fromValue, object newValue)
        {
            Timeline timeline;
            if (typeof(double).GetTypeInfo().IsAssignableFrom(propertyInfo.PropertyType.GetTypeInfo()))
            {
                timeline = this.CreateDoubleAnimation((double)fromValue, (double)newValue);
            }
            else if (typeof(Color).GetTypeInfo().IsAssignableFrom(propertyInfo.PropertyType.GetTypeInfo()))
            {
                timeline = this.CreateColorAnimation((Color)fromValue, (Color)newValue);
            }
            else if (typeof(Point).GetTypeInfo().IsAssignableFrom(propertyInfo.PropertyType.GetTypeInfo()))
            {
                timeline = this.CreatePointAnimation((Point)fromValue, (Point)newValue);
            }
            else
            {
                timeline = this.CreateKeyFrameAnimation(fromValue, newValue);
            }

            timeline.Duration = Duration;

            var storyboard = new Storyboard {FillBehavior = FillBehavior.HoldEnd};
            storyboard.Children.Add(timeline);
            Storyboard.SetTarget(storyboard, (DependencyObject) Target);
            Storyboard.SetTargetProperty(storyboard, propertyInfo.Name);
            storyboard.Completed += (o, e) => propertyInfo.SetValue(Target, newValue, new object[0]);
            storyboard.Begin();
        }

        /// <summary>
        /// Creates a color animation
        /// </summary>
        /// <param name="fromValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private Timeline CreateColorAnimation(Color fromValue, Color newValue)
        {
            return new ColorAnimation
                {
                    From = fromValue,
                    To = newValue,
                    EasingFunction = Ease,
                };
        }

        /// <summary>
        /// Creates a double animation
        /// </summary>
        /// <param name="fromValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private Timeline CreateDoubleAnimation(double fromValue, double newValue)
        {
            return new DoubleAnimation
                {
                    From = fromValue,
                    To = newValue,
                    EasingFunction = Ease,
                };
        }

        /// <summary>
        /// Creates a generic keyframe animation
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="fromValue"></param>
        /// <returns></returns>
        private Timeline CreateKeyFrameAnimation(object newValue, object fromValue)
        {
            var frames = new ObjectAnimationUsingKeyFrames();

            frames.KeyFrames.Add(new DiscreteObjectKeyFrame
                                    {
                                        KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0L)),
                                        Value = fromValue
                                    });
            frames.KeyFrames.Add(new DiscreteObjectKeyFrame
                                    {
                                        KeyTime = KeyTime.FromTimeSpan(Duration.TimeSpan),
                                        Value = newValue
                                    });
            
            return frames;
        }

        /// <summary>
        /// Creates a Point animation
        /// </summary>
        /// <param name="fromValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private Timeline CreatePointAnimation(Point fromValue, Point newValue)
        {
            return new PointAnimation
                       {
                           From = fromValue,
                           To = newValue,
                           EasingFunction = Ease,
                       };
        }

        /// <summary>
        /// Returns the current property value for a given object/Property.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private static object GetCurrentPropertyValue(object target, PropertyInfo propertyInfo)
        {
            FrameworkElement element = target as FrameworkElement;
            object value = propertyInfo.GetValue(target, null);
            
            if ((element == null) || (propertyInfo.Name != "Width" && propertyInfo.Name != "Height"))
                return value;

            return !double.IsNaN((double) value)
                       ? value
                       : (propertyInfo.Name == "Width" ? element.ActualWidth : element.ActualHeight);
        }

        /// <summary>
        /// Invoke method - must be overridden
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {
            if (Target != null && !string.IsNullOrEmpty(this.PropertyName))
            {
                PropertyInfo property = LookForPropertyDeclaration(Target, PropertyName);
                ValidateProperty(property);
                object newValue = this.Value;

                Exception innerException = null;
                try
                {
                    if (this.Duration.HasTimeSpan)
                    {
                        if (!typeof(DependencyObject).GetTypeInfo().IsAssignableFrom(Target.GetType().GetTypeInfo()))
                            throw new InvalidOperationException("Cannot animate property value");

                        object currentPropertyValue = GetCurrentPropertyValue(base.Target, property);
                        AnimatePropertyChange(property, currentPropertyValue, newValue);
                    }
                    else
                    {
                        property.SetValue(base.Target, newValue, new object[0]);
                    }
                }
                catch (Exception ex)
                {
                    innerException = ex;
                }

                if (innerException != null)
                    throw new ArgumentException("Cannot set property value for " + PropertyName + " to " + newValue, innerException);
            }
        }

        /// <summary>
        /// This searches the type for a given property - including ancestor classes.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private PropertyInfo LookForPropertyDeclaration(object target, string propertyName)
        {
            Type currentType = target.GetType();
            while (currentType != typeof(object))
            {
                TypeInfo typeInfo = currentType.GetTypeInfo();
                PropertyInfo propInfo = typeInfo.GetDeclaredProperty(propertyName);
                if (propInfo != null)
                    return propInfo;

                currentType = typeInfo.BaseType;
            }

            return null;
        }

        /// <summary>
        /// Validates the selected property
        /// </summary>
        /// <param name="propertyInfo"></param>
        private void ValidateProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentException("Cannot locate property name.");
            if (!propertyInfo.CanWrite)
                throw new ArgumentException("Cannot change read-only property.");
        }
    }


}
