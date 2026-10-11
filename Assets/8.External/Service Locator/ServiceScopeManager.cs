using System;

internal sealed class ServiceScopeManager
{
    internal ServiceScope GlobalScope { get; }
    internal ServiceScope CurrentScope { get; private set; }

    internal ServiceScopeManager()
    {
        GlobalScope = new ServiceScope("Global");
        CurrentScope = GlobalScope;
    }

    internal void OpenScope(string name)
    {
        var scope = new ServiceScope(name, CurrentScope ?? GlobalScope);
        CurrentScope = scope;
    }

    internal ServiceScope CloseCurrentScope()
    {
        ServiceScope scope = CurrentScope;

        if (scope == GlobalScope)
        {
            throw new InvalidOperationException("Global service scope cannot be closed.");
        }

        CurrentScope = scope.Parent ?? GlobalScope;

        return scope;
    }
}
