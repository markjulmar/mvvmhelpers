namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// This attribute is used to inject the caller member name in C# 5.0
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class CallerMemberNameAttribute : Attribute
    {
    }

    /// <summary>
    /// This attribute is used to inject the caller file path in C# 5.0
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class CallerFilePathAttribute : Attribute
    {
    }

    /// <summary>
    /// This attribute is is used to inject the caller line # in C# 5.0
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class CallerLineNumberAttribute : Attribute
    {
    }
}
