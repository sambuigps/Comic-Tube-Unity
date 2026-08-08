public enum ApiErrorType
{
    None,
    Network,
    Api
}

public class ApiResponse<T>
{
    public bool success;
    public T data;
    public ApiErrorType errorType;
    public string message; 
}