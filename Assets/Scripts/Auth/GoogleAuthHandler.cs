using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleAuthRequest
{
    public string platformType;
    public string idToken;

    public GoogleAuthRequest(string idToken)
    {
        platformType = SO.env.platformType;
        this.idToken = idToken;
    }
}

public static class GoogleAuthHandler
{
    private static Action<ApiResponse<AuthResponse>> callback;

    public static void Login(Action<ApiResponse<AuthResponse>> m_callback)
    {
        callback = m_callback;
        _ = RunLogin();
    }

    #region browser
    private static async Task RunLogin()
    {
        HttpListener listener;
        string redirectUri;

        try
        {
            int port = GetFreePort();
            redirectUri = $"http://127.0.0.1:{port}/";
            listener = new HttpListener();
            listener.Prefixes.Add(redirectUri);
            listener.Start();
        }
        catch (Exception e)
        {
            OnBrowserExit(false, null, "Could not start local listener: " + e.Message);
            return;
        }

        string verifier = GenerateCodeVerifier();
        string challenge = GenerateCodeChallenge(verifier);

        string authUrl =
            "https://accounts.google.com/o/oauth2/v2/auth" +
            $"?client_id={Uri.EscapeDataString(SO.env.GoogleClientId)}" +
            $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
            "&response_type=code" +
            "&scope=openid%20email%20profile" +
            $"&code_challenge={challenge}" +
            "&code_challenge_method=S256";

        Application.OpenURL(authUrl);

        (string code, string listenError) = await Task.Run(() => WaitForRedirect(listener));
        listener.Close();

        if (listenError != null)
        {
            OnBrowserExit(false, null, listenError);
            return;
        }

        if (string.IsNullOrEmpty(code))
        {
            OnBrowserExit(false, null, "Google sign-in was cancelled or failed.");
            return;
        }

        await ExchangeCodeForIdToken(code, verifier, redirectUri);
    }

    private static (string code, string error) WaitForRedirect(HttpListener listener)
    {
        try
        {
            HttpListenerContext context = listener.GetContext(); // blocks until browser hits redirect
            var query = HttpUtility.ParseQueryString(context.Request.Url.Query);
            string code = query["code"];
            string error = query["error"];

            string message = error == null
                ? "Signed in successfully. You can close this tab and return to the app."
                : "Sign-in failed — you can close this tab.";

            byte[] buffer = Encoding.UTF8.GetBytes($"<html><body>{message}</body></html>");
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();

            return error == null ? (code, null) : (null, "Google returned error: " + error);
        }
        catch (Exception e)
        {
            return (null, "Listener error: " + e.Message);
        }
    }

    private static async Task ExchangeCodeForIdToken(string code, string verifier, string redirectUri)
    {
        WWWForm form = new WWWForm();
        form.AddField("client_id", SO.env.GoogleClientId);
        form.AddField("client_secret", SO.env.GoogleClientSecret);
        form.AddField("code", code);
        form.AddField("code_verifier", verifier);
        form.AddField("grant_type", "authorization_code");
        form.AddField("redirect_uri", redirectUri);

        using UnityWebRequest req = UnityWebRequest.Post("https://oauth2.googleapis.com/token", form);
        var op = req.SendWebRequest();
        while (!op.isDone) await Task.Yield();

        if (req.result != UnityWebRequest.Result.Success)
        {
            OnBrowserExit(false, null, "Token exchange failed: " + req.downloadHandler.text);
            return;
        }

        TokenResponse token;
        try
        {
            token = JsonUtility.FromJson<TokenResponse>(req.downloadHandler.text);
        }
        catch (Exception e)
        {
            OnBrowserExit(false, null, "Failed to parse token response: " + e.Message);
            return;
        }

        if (string.IsNullOrEmpty(token.id_token))
        {
            OnBrowserExit(false, null, "No id_token in Google's response: " + req.downloadHandler.text);
            return;
        }

        OnBrowserExit(true, token.id_token, null);
    }

    private static int GetFreePort()
    {
        TcpListener l = new TcpListener(IPAddress.Loopback, 0);
        l.Start();
        int port = ((IPEndPoint)l.LocalEndpoint).Port;
        l.Stop();
        return port;
    }

    private static string GenerateCodeVerifier()
    {
        byte[] bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Base64UrlEncode(bytes);
    }

    private static string GenerateCodeChallenge(string verifier)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(verifier));
        return Base64UrlEncode(hash);
    }

    private static string Base64UrlEncode(byte[] input) =>
        Convert.ToBase64String(input).Replace('+', '-').Replace('/', '_').TrimEnd('=');

    [Serializable]
    private class TokenResponse
    {
        public string id_token;
        public string access_token;
    }
    #endregion

    private static void OnBrowserExit(bool success, string idToken, string error)
    {
        if (!success)
        {
            var response = new ApiResponse<AuthResponse>();
            response.success = false;
            callback?.Invoke(response);
            return;
        }

        var body = new GoogleAuthRequest(idToken);

        ApiHandler.Send<GoogleAuthRequest, AuthResponse>(
            SO.env.GoogleAuthEndpoint,
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