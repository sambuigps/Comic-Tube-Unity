using System;

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
            }
        );
    }
}
