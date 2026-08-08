using System;
using System.Collections;

[System.Serializable]
public class LoginRequest
{
    public string platformType;
    public string emailOrUsername;
    public string password;
    public LoginRequest(string emailOrUsername, string password)
    {
        this.emailOrUsername = emailOrUsername;
        this.password = password;
        platformType = SO.env.platformType;
    }
}

[System.Serializable]
public class LoginResponse
{
    public User user;
    public string accessToken;
    public string refreshToken;
}

public static class LoginHandler
{
    public static void Login(string emailOrUsername, string password, Action<ApiResponse<LoginResponse>> callback)
    {
        var body = new LoginRequest(emailOrUsername, password);

        ApiHandler.Send<LoginRequest, LoginResponse>(
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
