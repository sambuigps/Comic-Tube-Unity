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

    public string LoginEndpoint => apiBaseUrl + authEndpoint + loginEndpoint;
    public string SignupEndpoint => apiBaseUrl + authEndpoint + signupEndpoint;
    public string LogoutEndpoint => apiBaseUrl + authEndpoint + logoutEndpoint;

    [Header("constants")]
    public string platformType;

}
