using UnityEngine;

[CreateAssetMenu(fileName = "SaveDataKeysSO", menuName = "Scriptable Objects/SaveDataKeysSO")]
public class SaveDataKeysSO : ScriptableObject
{
    public string AccessTokenKey = "AccessToken";
    public string RefreshTokenKey = "RefreshToken";
}
