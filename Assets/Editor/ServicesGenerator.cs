using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;

public static class ServicesGenerator
{
    private const string ServicesFile = "Assets/Scripts/Services.cs";
    private const string folder = "Assets/Scripts/Services";

    public static void Generate(ServicesSO data)
    {
        string code = File.ReadAllText(ServicesFile);
        code = ReplaceRegion(code, "GENERATED_FIELDS_AND_GETTERS", GenerateFieldsAndGetters(data), "    ");
        code = ReplaceRegion(code, "GENERATED_DICTIONARY", GenerateDictionary(data), "        ");
        code = ReplaceRegion(code, "GENERATED_INIT", GenerateInit(data), "        ");
        File.WriteAllText(ServicesFile, code);
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static string GetDataField(string dataType)
    {
        return dataType.ToLower();
    }

    private static string GetDataTypeName(string serviceName)
    {
        var serviceType = FindType(serviceName);
        if (serviceType == null) return null;

        var t = serviceType;
        while (t != null && t != typeof(object))
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(BaseService<>))
                return t.GetGenericArguments()[0].Name;
            t = t.BaseType;
        }
        return null;
    }

    private static Type FindType(string typeName)
    {
        var candidates = new System.Collections.Generic.List<Type>();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;
            try { types = assembly.GetTypes(); }
            catch (ReflectionTypeLoadException e) { types = e.Types.Where(t => t != null).ToArray(); }
            candidates.AddRange(types.Where(t => t != null && t.Name == typeName));
        }

        if (candidates.Count == 0) return null;
        if (candidates.Count == 1) return candidates[0];

        var preferred = candidates.FirstOrDefault(DerivesFromBaseService);
        if (preferred != null) return preferred;

        return candidates.FirstOrDefault(t =>
            t.Namespace?.StartsWith("UnityEditor") != true &&
            t.Namespace?.StartsWith("UnityEngine") != true)
            ?? candidates[0];
    }

    private static bool DerivesFromBaseService(Type t)
    {
        while (t != null && t != typeof(object))
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(BaseService<>))
                return true;
            t = t.BaseType;
        }
        return false;
    }

    private static string GenerateFieldsAndGetters(ServicesSO data)
    {
        StringBuilder sb = new();
        foreach (var s in data.services)
        {
            var dataType = GetDataTypeName(s.service);
            if (dataType != null)
                sb.AppendLine($"    [SerializeField] {dataType} {GetDataField(dataType)};");
        }
        foreach (var s in data.services)
        {
            sb.AppendLine($"    public static {s.service} {s.getter} => GetService<{s.service}>();");
        }
        return sb.ToString();
    }

    private static string GenerateDictionary(ServicesSO data)
    {
        StringBuilder sb = new();
        foreach (var s in data.services)
        {
            sb.AppendLine($"        {{ typeof({s.service}), new {s.service}() }},");
        }
        return sb.ToString();
    }

    private static string GenerateInit(ServicesSO data)
    {
        StringBuilder sb = new();
        foreach (var s in data.services)
        {
            var dataType = GetDataTypeName(s.service);
            var arg = dataType != null ? GetDataField(dataType) : "";
            sb.AppendLine($"        GetService<{s.service}>().InitData({arg});");
        }
        return sb.ToString();
    }

    private static string ReplaceRegion(string text, string regionName, string replacement, string endIndent = "")
    {
        string startMarker = $"#region {regionName}";
        string endMarker = "#endregion";
        int start = text.IndexOf(startMarker);
        if (start == -1) return text;
        int end = text.IndexOf(endMarker, start);
        if (end == -1) return text;
        start += startMarker.Length;
        return text.Substring(0, start) + "\n" + replacement + endIndent + text.Substring(end);
    }

    private const string ServiceTemplate =
    @"using UnityEngine;
    
    [System.Serializable]
    public class {0}_Data : BaseDataClass
    {
    }
    
    public class {0} : BaseService<{0}_Data>
    {
    }
    ";

    public static void AddService(string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
            return;

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, serviceName + ".cs");

        if (!File.Exists(path))
        {
            File.WriteAllText(path, ServiceTemplate.Replace("{0}", serviceName));
            AssetDatabase.Refresh();
        }
    }

    public static void RemoveService(string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
            return;

        string path = Path.Combine(folder, serviceName + ".cs");

        if (File.Exists(path))
        {
            AssetDatabase.DeleteAsset(path);
        }
    }
}