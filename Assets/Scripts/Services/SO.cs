[System.Serializable]
public class SO_Data : BaseDataClass
{
    public envSO env;
    public SaveDataKeysSO saveDataKeys;
}

public class SO : BaseService<SO_Data>
{
    public static envSO env => Services.SO.data.env;
    public static SaveDataKeysSO saveDataKeys => Services.SO.data.saveDataKeys;
}
    