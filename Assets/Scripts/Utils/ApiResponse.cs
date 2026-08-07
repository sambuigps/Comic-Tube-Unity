[System.Serializable]
public class ApiResponse<T>
{
    public int statusCode;
    public T data;
    public string message;
    public bool success;
}
