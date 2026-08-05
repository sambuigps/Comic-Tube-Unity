[System.Serializable]
public class SO_Data : BaseDataClass
{
    public envSO env;
}

public class SO : BaseService<SO_Data>
{
    public static envSO env => Services.SO.data.env;
}
    