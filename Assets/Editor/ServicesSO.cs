using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CreateAssetMenu(fileName = "ServicesSO", menuName = "Scriptable Objects/ServicesSO")]
public class ServicesSO : ScriptableObject
{
    [System.Serializable]
    public class ServiceEntry
    {
        [TypeNameConstraint(typeof(IService))]
        public string service;
        public string getter;
    }

    [SerializeField, EditorButton("AddService", "RemoveService")] string serviceName;
    private void AddService()
    {
        ServicesGenerator.AddService(serviceName);
        if (!services.Exists(x => x.service == serviceName))
        {
            services.Add(new ServiceEntry
            {
                service = serviceName,
                getter = serviceName
            });

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
    private void RemoveService()
    {
        ServicesGenerator.RemoveService(serviceName);

        services.RemoveAll(x => x.service == serviceName);

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
    [Space]
    [Space]

    [SerializeField, EditorButton("Generate")]
    public List<ServiceEntry> services = new();
    private void Generate()
    {
        ServicesGenerator.Generate(this);
    }
    
    private void OnValidate()
    {
        foreach (var s in services)
        {
            if (string.IsNullOrEmpty(s.getter) && !string.IsNullOrEmpty(s.service))
            {
                s.getter = s.service;
            }
        }
    }
}
#endif