using UnityEngine;

[CreateAssetMenu(fileName = "envSO", menuName = "Scriptable Objects/envSO")]
public class envSO : ScriptableObject
{
    [Header("API")]
    public string apiBaseUrl;

    [Header("AuthEndpoints")]
    [SerializeField] string authEndpoint;
    [SerializeField] string loginEndpoint;
    [SerializeField] string signupEndpoint;
    [SerializeField] string logoutEndpoint;
    [SerializeField] string getCurrUserEndpoint;
    [SerializeField] string refreshAccessTokenEndpoint;
    [SerializeField] string verifyOtpEndpoint;
    [SerializeField] string googleAuthEndpoint;

    #region full endpoint generators

    private string AuthEndpoint => apiBaseUrl + authEndpoint;

    public string LoginEndpoint => AuthEndpoint + loginEndpoint;
    public string SignupEndpoint => AuthEndpoint + signupEndpoint;
    public string LogoutEndpoint => AuthEndpoint + logoutEndpoint;
    public string GetCurrUserEndpoint => AuthEndpoint + getCurrUserEndpoint;
    public string RefreshAccessTokenEndpoint => AuthEndpoint + refreshAccessTokenEndpoint;
    public string VerifyOtpEndpoint => AuthEndpoint + verifyOtpEndpoint;
    public string GoogleAuthEndpoint => AuthEndpoint + googleAuthEndpoint;
    #endregion

    [Header("Auth IDs")]
    [SerializeField] string googleClientId_Web;
    [SerializeField] string googleClientSecret_Web;
    [SerializeField] string googleClientId_App;
    [SerializeField] string googleClientSecret_App;

    public string GoogleClientId => platformType=="web" ? googleClientId_Web : googleClientId_App;
    public string GoogleClientSecret => platformType == "web" ? googleClientSecret_Web : googleClientSecret_App;

    [Header("constants")]
    public string platformType => GetPlatformType();

    string GetPlatformType()
    {
        #if UNITY_WEBGL
        return "web";
        #else
        return "app";
        #endif
    }

}
