using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

public static class ButtonEditorDrawer
{
    public static void Draw(SerializedObject serializedObject, Object target)
    {
        serializedObject.Update();
        System.Type type = target.GetType();

        List<MethodInfo> standaloneMethods = new List<MethodInfo>();
        foreach (MethodInfo method in type.GetMethods(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            EditorButtonAttribute attr = method.GetCustomAttribute<EditorButtonAttribute>();
            if (attr != null)
                standaloneMethods.Add(method);
        }

        SerializedProperty prop = serializedObject.GetIterator();
        bool enterChildren = true;
        while (prop.NextVisible(enterChildren))
        {
            enterChildren = false;
            if (prop.propertyPath == "m_Script")
            {
                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.PropertyField(prop);
                continue;
            }
            EditorGUILayout.PropertyField(prop, true);
            FieldInfo field = type.GetField(prop.name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null)
            {
                EditorGUILayout.Space(16);
                continue;
            }
            EditorButtonAttribute attr = field.GetCustomAttribute<EditorButtonAttribute>();
            if (attr != null)
            {
                foreach (string functionName in attr.FunctionNames)
                {
                    MethodInfo method = type.GetMethod(functionName,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (method != null)
                    {
                        EditorGUILayout.Space(2);
                        if (GUILayout.Button(functionName))
                            method.Invoke(target, null);
                    }
                }
            }
        }

        if (standaloneMethods.Count > 0)
        {
            foreach (MethodInfo method in standaloneMethods)
            {
                EditorButtonAttribute attr = method.GetCustomAttribute<EditorButtonAttribute>();
                foreach (string functionName in attr.FunctionNames)
                {
                    if (GUILayout.Button(functionName))
                        method.Invoke(target, null);
                    EditorGUILayout.Space(2);
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(MonoBehaviour), true)]
public class MonoBehaviourButtonEditor : Editor
{
    public override void OnInspectorGUI() => ButtonEditorDrawer.Draw(serializedObject, target);
}

[CustomEditor(typeof(ScriptableObject), true)]
public class ScriptableObjectButtonEditor : Editor
{
    public override void OnInspectorGUI() => ButtonEditorDrawer.Draw(serializedObject, target);
}