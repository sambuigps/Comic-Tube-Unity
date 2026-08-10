using System;

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

public static class SignUpHandler
{
    public static void Signup(string email, string username, string password, Action<ApiResponse<object>> callback)
    {
        var body = new SignUpRequest(email, username, password);

        ApiHandler.Send<SignUpRequest, object>(
            SO.env.SignupEndpoint,
            "POST",
            body,
            (response) =>
            {
                if (response.success)
                {
                    Services.Session.unverifiedEmail = email;
                }
                callback?.Invoke(response);
            }
        );
    }
}
