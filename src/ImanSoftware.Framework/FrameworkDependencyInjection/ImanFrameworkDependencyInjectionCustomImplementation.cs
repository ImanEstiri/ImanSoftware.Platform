

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
}
