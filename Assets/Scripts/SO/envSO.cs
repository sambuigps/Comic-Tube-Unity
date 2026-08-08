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

    public string LoginEndpoint => apiBaseUrl + authEndpoint + loginEndpoint;
    public string SignupEndpoint => apiBaseUrl + authEndpoint + signupEndpoint;
    public string LogoutEndpoint => apiBaseUrl + authEndpoint + logoutEndpoint;
    public string GetCurrUserEndpoint => apiBaseUrl + authEndpoint + getCurrUserEndpoint;
    public string RefreshAccessTokenEndpoint => apiBaseUrl + authEndpoint + refreshAccessTokenEndpoint;

    [Header("constants")]
    public string platformType;

}
