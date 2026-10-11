using System;

[AttributeUsage(AttributeTargets.Class)]
public class DummyServiceAttribute : Attribute
{
    public Type InterfaceType { get; private set; }

    public DummyServiceAttribute(Type interfaceType)
    {
        InterfaceType = interfaceType;
    }
}