using UnityEngine;

[CreateAssetMenu(fileName = "envSO", menuName = "Scriptable Objects/envSO")]
public class envSO : ScriptableObject
{
    [Header("API")]
    public string apiBaseUrl;

    [Header("AuthEndpoints")]
    public string authEndpoint;
    public string loginEndpoint;
    public string signupEndpoint;
    public string logoutEndpoint;
}
