using System;
using System.Collections;

[System.Serializable]
public class SignUpRequest
{
    public string platformType;
    public string username;
    public string email;
    public string password;

    public SignUpRequest(string email, string username, string password = "")
    {
        this.email = email;
        this.password = password;
        this.username = username;
        platformType = SO.env.platformType;
    }
}

[System.Serializable]
public class SignUpResponse
{
    public User user;
    public string accessToken;
    public string refreshToken;
}

public static class SignUpHandler
{
    public static void Signup(string email, string username, string password, Action<ApiResponse<SignUpResponse>> callback)
    {
        var body = new SignUpRequest(email, username, password);

        ApiHandler.Send<SignUpRequest, SignUpResponse>(
            SO.env.SignupEndpoint,
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
