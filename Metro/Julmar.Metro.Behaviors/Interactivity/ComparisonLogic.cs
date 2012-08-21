using System;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This performs specific comparison logic
    /// </summary>
    internal static class ComparisonLogic
    {
        // Methods
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

        internal static bool EvaluateImpl(object leftOperand, ComparisonConditionType operatorType, object rightOperand)
        {
            if (leftOperand != null)
            {
                Type type = leftOperand.GetType();
                if (rightOperand != null)
                {
                    if (rightOperand.GetType() != type)
                    {
                        try
                        {
                            if (rightOperand is string)
                            {
                                string value = rightOperand.ToString();
                                if (type == typeof(byte))
                                    rightOperand = byte.Parse(value);
                                else if (type == typeof(sbyte))
                                    rightOperand = sbyte.Parse(value);
                                else if (type == typeof(short))
                                    rightOperand = short.Parse(value);
                                else if (type == typeof(ushort))
                                    rightOperand = ushort.Parse(value);
                                else if (type == typeof(int))
                                    rightOperand = int.Parse(value);
                                else if (type == typeof(uint))
                                    rightOperand = uint.Parse(value);
                                else if (type == typeof(long))
                                    rightOperand = long.Parse(value);
                                else if (type == typeof(ulong))
                                    rightOperand = ulong.Parse(value);
                                else if (type == typeof(double))
                                    rightOperand = double.Parse(value);
                                else if (type == typeof(decimal))
                                    rightOperand = decimal.Parse(value);
                                else if (type == typeof(DateTime))
                                    rightOperand = DateTime.Parse(value);
                                else if (type == typeof(float))
                                    rightOperand = float.Parse(value);
                                else if (type == typeof(bool))
                                    rightOperand = bool.Parse(value);
                            }
                            else
                            {
                                if (type == typeof(byte))
                                    rightOperand = Convert.ToByte(rightOperand);
                                else if (type == typeof(sbyte))
                                    rightOperand = Convert.ToSByte(rightOperand);
                                else if (type == typeof(short))
                                    rightOperand = Convert.ToInt16(rightOperand);
                                else if (type == typeof(ushort))
                                    rightOperand = Convert.ToUInt16(rightOperand);
                                else if (type == typeof(int))
                                    rightOperand = Convert.ToInt32(rightOperand);
                                else if (type == typeof(uint))
                                    rightOperand = Convert.ToUInt32(rightOperand);
                                else if (type == typeof(long))
                                    rightOperand = Convert.ToInt64(rightOperand);
                                else if (type == typeof(ulong))
                                    rightOperand = Convert.ToUInt64(rightOperand);
                                else if (type == typeof(string))
                                    rightOperand = rightOperand.ToString();
                                else if (type == typeof(double))
                                    rightOperand = Convert.ToDouble(rightOperand);
                                else if (type == typeof(decimal))
                                    rightOperand = Convert.ToDecimal(rightOperand);
                                else if (type == typeof(DateTime))
                                    rightOperand = Convert.ToDateTime(rightOperand);
                                else if (type == typeof(float))
                                    rightOperand = Convert.ToSingle(rightOperand);
                                else if (type == typeof(bool))
                                    rightOperand = Convert.ToBoolean(rightOperand);
                            }
                        }
                        catch
                        {
                        }
                    }
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
    }

 

}
