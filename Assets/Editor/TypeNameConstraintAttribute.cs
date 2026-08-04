using System;
using UnityEngine;

public class TypeNameConstraintAttribute : PropertyAttribute
{
    public readonly Type baseType;
    public TypeNameConstraintAttribute(Type baseType)
    {
        this.baseType = baseType;
    }
}