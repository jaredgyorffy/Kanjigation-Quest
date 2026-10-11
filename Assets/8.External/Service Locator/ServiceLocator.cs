using System;
using System.Linq;
using System.Reflection;
using Core.Singleton;
using UnityEngine;

public class ServiceLocator : StandaloneSingleton<ServiceLocator>
{
    private ServiceScopeManager Scopes { get; } = new ServiceScopeManager();

    public T Get<T>() where T : ILocatableService
    {
        Type serviceType = typeof(T);
        ServiceScope currentScope = Scopes.CurrentScope;
        while (currentScope != null)
        {
            if (currentScope.TryGetService(serviceType, out ILocatableService service))
            {
                return (T)service;
            }

            currentScope = currentScope.Parent;
        }

        if (TryRegisterDummyService<T>())
        {
            Debug.LogWarning($"Missing {serviceType.Name} service was registered as a dummy service in the {Scopes.CurrentScope.Name} scope.");
            return Get<T>();
        }

        Debug.LogError($"Service of type {serviceType.Name} is not registered in the {Scopes.CurrentScope.Name} scope or any parent scope.");
        throw new InvalidOperationException();
    }

    private bool TryRegisterDummyService<T>() where T : ILocatableService
    {
        Type interfaceType = typeof(T);

        Type dummyServiceType = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract)
            .FirstOrDefault(type =>
            {
                var attr = type.GetCustomAttribute<DummyServiceAttribute>();
                return attr != null && attr.InterfaceType == interfaceType;
            });

        if (dummyServiceType == null)
        {
            return false;
        }

        var instance = Activator.CreateInstance(dummyServiceType);
        Register<T>((T)instance);
        return true;
    }

    public void Register<T>(T service) where T : ILocatableService
    {
        Type serviceType = typeof(T);
        ServiceScope currentScope = Scopes.CurrentScope;
        currentScope.AddService(serviceType, service);
    }

    public void OpenScope(string scopeName)
    {
        Scopes.OpenScope(scopeName);
    }

    public void CloseScope()
    {
        ServiceScope closedScope = Scopes.CloseCurrentScope();
        closedScope.Dispose();
    }

    public void CloseAllChildScopes()
    {
        while (Scopes.CurrentScope != Scopes.GlobalScope)
        {
            CloseScope();
        }
    }
}