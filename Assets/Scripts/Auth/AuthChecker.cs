using System;
using System.Collections.Generic;

public class RefreshAccessTokenRequest
{
    public string platformType;
    public string refreshToken;
    public RefreshAccessTokenRequest()
    {
        platformType = SO.env.platformType;
        refreshToken = Services.Save.GetRefreshToken();
    }
}

public class RefreshAccessTokenResponse
{
    public string refreshToken;
    public string accessToken;
}

public static class AuthChecker
{
    public static void Check(Action<bool> callback)
    {
        FetchCurrUser(onSuccess: () => callback?.Invoke(true), onFail: () =>
        {
            RefreshAccessToken((refreshResult) =>
            {
                if (!refreshResult.success)
                {
                    Services.Session.ClearUser();
                    callback?.Invoke(false);
                    return;
                }

                Services.Save.SetAccessToken(refreshResult.data.accessToken);
                Services.Save.SetRefreshToken(refreshResult.data.refreshToken);

                // Retry once with the fresh token. If this also fails, give up.
                FetchCurrUser(
                    onSuccess: () => callback?.Invoke(true),
                    onFail: () =>
                    {
                        Services.Session.ClearUser();
                        callback?.Invoke(false);
                    });
            });
        });
    }

    private static void FetchCurrUser(Action onSuccess, Action onFail)
    {
        string accessToken = Services.Save.GetAccessToken();
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            onFail?.Invoke();
            return;
        }

        var headers = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {accessToken}" }
        };

        ApiHandler.Send<object, User>(
            SO.env.GetCurrUserEndpoint,
            "GET",
            null,
            (result) =>
            {
                if (result.success)
                {
                    Services.Session.loggedInUser = result.data;
                    onSuccess?.Invoke();
                }
                else
                {
                    onFail?.Invoke();
                }
            },
            headers
        );
    }

    private static void RefreshAccessToken(Action<ApiResponse<RefreshAccessTokenResponse>> callback)
    {
        string refreshToken = Services.Save.GetRefreshToken();
        if (string.IsNullOrEmpty(refreshToken))
        {
            callback?.Invoke(new ApiResponse<RefreshAccessTokenResponse>
            {
                success = false,
                message = "No stored session"
            });
            return;
        }

        var body = new RefreshAccessTokenRequest();

        ApiHandler.Send<RefreshAccessTokenRequest, RefreshAccessTokenResponse>(
            SO.env.RefreshAccessTokenEndpoint,
            "POST",
            body,
            callback
        );
    }
}