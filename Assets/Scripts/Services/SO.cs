[System.Serializable]
public class SO_Data : BaseDataClass
{
    public envSO env;
    public SaveDataKeysSO saveDataKeys;
    public PrefabsSO prefabs;
}

public class SO : BaseService<SO_Data>
{
    public static envSO env => Services.SO.data.env;
    public static SaveDataKeysSO saveDataKeys => Services.SO.data.saveDataKeys;
    public static PrefabsSO prefabs => Services.SO.data.prefabs;
}
    