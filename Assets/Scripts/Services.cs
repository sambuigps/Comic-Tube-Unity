using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Services : MonoBehaviour
{
    private static Services Instance;
    private static Transform _transform;
    public static Transform Transform => _transform;

    #region GENERATED_FIELDS_AND_GETTERS
    [SerializeField] SO_Data so_data;
    [SerializeField] SaveDataManager_Data savedatamanager_data;
    [SerializeField] UI_Manager_Data ui_manager_data;
    public static SO SO => GetService<SO>();
    public static SaveDataManager Save => GetService<SaveDataManager>();
    public static UI_Manager UI => GetService<UI_Manager>();
    #endregion

    private static Dictionary<Type, IService> services = new Dictionary<Type, IService>()
    {
        #region GENERATED_DICTIONARY
        { typeof(SO), new SO() },
        { typeof(SaveDataManager), new SaveDataManager() },
        { typeof(UI_Manager), new UI_Manager() },
        #endregion
    };
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _transform = transform;

        #region GENERATED_INIT
        GetService<SO>().InitData(so_data);
        GetService<SaveDataManager>().InitData(savedatamanager_data);
        GetService<UI_Manager>().InitData(ui_manager_data);
        #endregion

        foreach (var service in services)
        {
            service.Value.Awake();
        }
    }

    private static T GetService<T>() where T : IService
    {
        return (T)services[typeof(T)];
    }

    #region Link each unity function
    private void Start()
    {
        foreach (var service in services)
        {
            service.Value.Start();
        }
    }

    private void Update()
    {
        foreach (var service in services)
        {
            service.Value.Update();
        }
    }

    private void LateUpdate()
    {
        foreach (var service in services)
        {
            service.Value.LateUpdate();
        }
    }
    #endregion

    #region Input Validation
    #if UNITY_EDITOR
    private void OnValidate()
    {
        UI_Type[] values = (UI_Type[])Enum.GetValues(typeof(UI_Type));

        Dictionary<UI_Type, UI_Menu> existing = new();

        foreach (var menu in ui_manager_data.allMenus)
        {
            if (!existing.ContainsKey(menu.type))
                existing.Add(menu.type, menu);
        }

        ui_manager_data.allMenus.Clear();

        foreach (var type in values)
        {
            if (existing.TryGetValue(type, out var menu))
                ui_manager_data.allMenus.Add(menu);
            else
                ui_manager_data.allMenus.Add(new UI_Menu { type = type });
        }
        ui_manager_data.allMenus[0] = new UI_Menu(UI_Type.None, null);
    }
    #endif
    #endregion

    #region utils
    public static Coroutine RunCoroutine(IEnumerator routine)
    {
        return Instance.StartCoroutine(routine);
    }

    public static void StopRunningCoroutine(Coroutine coroutine)
    {
        Instance.StopCoroutine(coroutine);
    }
    #endregion
}

public abstract class IService
{
    public virtual void Awake() { }
    public virtual void Start() { }
    public virtual void Update() { }
    public virtual void LateUpdate() { }
}

public abstract class BaseService<T>: IService where T : BaseDataClass
{
    protected T data;
    public virtual void InitData(T data)
    {
        this.data = data;
    }
}

public abstract class BaseDataClass
{

}