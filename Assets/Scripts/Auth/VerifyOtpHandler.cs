using System;
using UnityEngine;

[System.Serializable]
public class VerifyOtpRequest
{
    public string platformType;
    public string email;
    public string otp;

    public VerifyOtpRequest(string email, string otp)
    {
        this.email = email;
        this.otp = otp;
        platformType = SO.env.platformType;
    }
}

[System.Serializable]
public class VerifyOtpResponse
{
    public User user;
    public string accessToken;
    public string refreshToken;
}

public static class VerifyOtpHandler
{
    public static void VerifyOtp(string otp, Action<ApiResponse<VerifyOtpResponse>> callback)
    {
        var body = new VerifyOtpRequest(Services.Session.unverifiedEmail, otp);

        ApiHandler.Send<VerifyOtpRequest, VerifyOtpResponse >(
            SO.env.VerifyOtpEndpoint,
            "POST",
            body,
            (response) =>
            {
                if (response.success)
                {
                    Services.Session.CacheUser(response.data.user, response.data.accessToken, response.data.refreshToken);
                    Services.Session.unverifiedEmail = null;
                }
                callback?.Invoke(response);
            }
        );
    }
}
