using System;
using Microsoft.Xaml.Interactions.Core;

namespace JulMar.Windows.Interactivity.Internal
{
    /// <summary>
    /// This performs specific comparison logic
    /// </summary>
    internal static class ComparisonLogic
    {
        /// <summary>
        /// Method to compare two objects
        /// </summary>
        /// <param name="leftOperand"></param>
        /// <param name="operatorType"></param>
        /// <param name="rightOperand"></param>
        /// <returns></returns>
        internal static bool Evaluate(object leftOperand, ComparisonConditionType operatorType, object rightOperand)
        {
            if (leftOperand != null)
            {
                if (rightOperand != null)
                {
                    object newValue = TypeConverter.Convert(leftOperand.GetType(), rightOperand);
                    if (newValue != null)
                        rightOperand = newValue;
                }
            }

            IComparable comparable = leftOperand as IComparable;
            IComparable comparable2 = rightOperand as IComparable;

            if ((comparable != null) && (comparable2 != null))
            {
                return EvaluateComparable(comparable, operatorType, comparable2);
            }

            switch (operatorType)
            {
                case ComparisonConditionType.Equal:
                    return Equals(leftOperand, rightOperand);

                case ComparisonConditionType.NotEqual:
                    return !Equals(leftOperand, rightOperand);

                case ComparisonConditionType.LessThan:
                case ComparisonConditionType.LessThanOrEqual:
                case ComparisonConditionType.GreaterThan:
                case ComparisonConditionType.GreaterThanOrEqual:
                    if ((comparable == null) && (comparable2 == null))
                        throw new ArgumentException("Operands must implement IComparable");
                    if (comparable == null)
                        throw new ArgumentException("Invalid left operand - must implement IComparable");
                    throw new ArgumentException("Invalid right operand - must implement IComparable.");
            }

            return false;
        }

        /// <summary>
        /// Method to compare two operands using IComparable
        /// </summary>
        /// <param name="leftOperand"></param>
        /// <param name="operatorType"></param>
        /// <param name="rightOperand"></param>
        /// <returns></returns>
        private static bool EvaluateComparable(IComparable leftOperand, ComparisonConditionType operatorType, IComparable rightOperand)
        {
            object value = null;
            try
            {
                value = Convert.ChangeType(rightOperand, leftOperand.GetType());
            }
            catch (Exception)
            {
            }

            if (value == null)
            {
                return (operatorType == ComparisonConditionType.NotEqual);
            }

            int num = leftOperand.CompareTo(value);
            switch (operatorType)
            {
                case ComparisonConditionType.Equal:
                    return (num == 0);

                case ComparisonConditionType.NotEqual:
                    return (num != 0);

                case ComparisonConditionType.LessThan:
                    return (num < 0);

                case ComparisonConditionType.LessThanOrEqual:
                    return (num <= 0);

                case ComparisonConditionType.GreaterThan:
                    return (num > 0);

                case ComparisonConditionType.GreaterThanOrEqual:
                    return (num >= 0);
            }
            return false;
        }
    }

 

}
