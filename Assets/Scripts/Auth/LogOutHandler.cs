using System;
using System.Collections.Generic;

[System.Serializable]
public class LogOutRequest
{
    public string platformType;
    public LogOutRequest()
    {
        platformType = SO.env.platformType;
    }
}

[System.Serializable]
public class LogOutResponse
{
}

public static class LogOutHandler
{
    public static void LogOut(Action<ApiResponse<LogOutResponse>> callback)
    {
        if (Services.Session.loggedInUser == null)
        {
            callback?.Invoke(new ApiResponse<LogOutResponse>
            {
                success = false,
                message = "No active session"
            });
            return;
        }

        var body = new LogOutRequest();

        var headers = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {Services.Save.GetAccessToken()}" }
        };

        ApiHandler.Send<LogOutRequest, LogOutResponse>(
            SO.env.LogoutEndpoint,
            "POST",
            body,
            (response) =>
            {
                if (response.success)
                {
                    Services.Session.ClearUser();
                }
                callback?.Invoke(response);
            },
            headers
        );
    }
}
