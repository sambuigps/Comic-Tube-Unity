#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TypeNameConstraintAttribute))]
public class TypeNameConstraintDrawer : PropertyDrawer
{
    // simple cache so we don't re-reflect every OnGUI call
    private static readonly Dictionary<Type, string[]> cache = new();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var constraint = (TypeNameConstraintAttribute)attribute;

        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "Use with string field only");
            return;
        }

        string[] options = GetOptions(constraint.baseType);

        if (options.Length == 0)
        {
            EditorGUI.LabelField(position, label.text, $"No types found deriving from {constraint.baseType.Name}");
            return;
        }

        EditorGUI.BeginProperty(position, label, property);

        int currentIndex = Array.IndexOf(options, property.stringValue);
        if (currentIndex < 0) currentIndex = 0; // fall back to first option if stale/empty

        EditorGUI.BeginChangeCheck();
        int newIndex = EditorGUI.Popup(position, label.text, currentIndex, options);
        if (EditorGUI.EndChangeCheck())
        {
            property.stringValue = options[newIndex];
        }

        EditorGUI.EndProperty();
    }

    private static string[] GetOptions(Type baseType)
    {
        if (cache.TryGetValue(baseType, out var cached))
            return cached;

        List<string> names = new();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;
            try { types = assembly.GetTypes(); }
            catch (ReflectionTypeLoadException e) { types = e.Types.Where(t => t != null).ToArray(); }

            foreach (var type in types)
            {
                if (type == null || type.IsAbstract || type.IsInterface) continue;
                if (type == baseType) continue;
                if (!baseType.IsAssignableFrom(type)) continue;

                names.Add(type.Name);
            }
        }

        names.Sort();
        var result = names.ToArray();
        cache[baseType] = result;
        return result;
    }

    // call this (e.g. from a menu item) if you add new classes and the dropdown
    // doesn't pick them up without an editor restart / recompile
    [MenuItem("Tools/Type Name Constraint/Clear Cache")]
    private static void ClearCache()
    {
        cache.Clear();
    }
}
#endif