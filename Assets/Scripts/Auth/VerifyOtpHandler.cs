using System;

[System.Serializable]
public class VerifyOtpRequest
{
    public string email;
    public string otp;

    public VerifyOtpRequest(string email, string otp)
    {
        this.email = email;
        this.otp = otp;
    }
}

public static class VerifyOtpHandler
{
    public static void VerifyOtp(string otp, Action<ApiResponse<AuthResponse>> callback)
    {
        var body = new VerifyOtpRequest(Services.Session.unverifiedEmail, otp);

        ApiHandler.Send<VerifyOtpRequest, AuthResponse >(
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
