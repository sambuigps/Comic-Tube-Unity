using System;

[System.Serializable]
public class LoginRequest
{
    public string emailOrUsername;
    public string password;
    public LoginRequest(string emailOrUsername, string password)
    {
        this.emailOrUsername = emailOrUsername;
        this.password = password;
    }
}

[System.Serializable]
public class AuthResponse
{
    public User user;
    public string accessToken;
    public string refreshToken;
}

public static class LoginHandler
{
    public static void Login(string emailOrUsername, string password, Action<ApiResponse<AuthResponse>> callback)
    {
        var body = new LoginRequest(emailOrUsername, password);

        ApiHandler.Send<LoginRequest, AuthResponse>(
            SO.env.LoginEndpoint,
            "POST",
            body,
            (response) =>
            {
                if (response.success)
                {
                    Services.Session.CacheUser(response.data.user, response.data.accessToken, response.data.refreshToken);
                }
                callback?.Invoke(response);
            }
        );
    }
}
