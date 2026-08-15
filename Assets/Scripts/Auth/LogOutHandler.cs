using System;

public static class LogOutHandler
{
    public static void LogOut(Action<ApiResponse<object>> callback)
    {
        if (Services.Session.loggedInUser == null)
        {
            callback?.Invoke(new ApiResponse<object>
            {
                success = false,
                message = "No active session"
            });
            return;
        }

        ApiHandler.Send<object, object>(
            SO.env.LogoutEndpoint,
            "POST",
            null,
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
