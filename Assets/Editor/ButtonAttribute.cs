using System;

public class EditorButtonAttribute : Attribute
{
    public string[] FunctionNames { get; }
    public EditorButtonAttribute(params string[] functionNames) => FunctionNames = functionNames;
}