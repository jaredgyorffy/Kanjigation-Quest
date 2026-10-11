using System;
using System.Collections.Generic;

internal sealed class ServiceScope : IDisposable
{
    internal string Name { get; }
    internal ServiceScope Parent { get; }
    private readonly Dictionary<Type, ILocatableService> _services = new();
    private readonly List<Type> _registrationOrder = new();

    internal ServiceScope(string name, ServiceScope parent = null)
    {
        Name = name;
        Parent = parent;
    }

    internal bool TryGetService(Type serviceType, out ILocatableService service)
    {
        return _services.TryGetValue(serviceType, out service);
    }

    internal void AddService(Type serviceType, ILocatableService service)
    {
        if (_services.ContainsKey(serviceType))
        {
            throw new InvalidOperationException($"Service of type {serviceType.Name} is already registered in the {Name} scope.");
        }

        _services.Add(serviceType, service);
        _registrationOrder.Add(serviceType);
    }

    public void Dispose()
    {
        List<Exception> disposalExceptions = null;

        for (int i = _registrationOrder.Count - 1; i >= 0; i--)
        {
            ILocatableService service = _services[_registrationOrder[i]];
            if (service is IDisposable disposable)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception exception)
                {
                    disposalExceptions ??= new List<Exception>();
                    disposalExceptions.Add(exception);
                }
            }
        }

        _registrationOrder.Clear();
        _services.Clear();

        if (disposalExceptions != null)
        {
            throw new AggregateException($"Failed to dispose services in the {Name} scope.", disposalExceptions);
        }
    }
}
