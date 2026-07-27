

namespace ImanSoftware.Framework.FrameworkDependencyInjection;

public class ImanFrameworkDependencyInjectionCustomImplementation<TInterface>
{
    private Type? customImplementation;

    public Type? CustomImplementation {
        get => customImplementation;
        set
        {
            if(value is not null && typeof(TInterface).IsAssignableFrom(value))
                customImplementation = value;
        }
    }


    /// <summary>
    /// Implicitly converts a Type to an instance of this class.
    /// Allows writing: options.JwtCustomImplementation = typeof(MyCustomJwtService);
    /// </summary>
    public static implicit operator ImanFrameworkDependencyInjectionCustomImplementation<TInterface>(Type type)
    {
        return new()
        {
            CustomImplementation = type
        };
    }
}
