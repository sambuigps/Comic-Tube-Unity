using UnityEngine;
    
[System.Serializable]
public class SaveDataManager_Data : BaseDataClass
{
}

public class SaveDataManager : BaseService<SaveDataManager_Data>
{
    #region Access and Refresh Tokens
    public void SetAccessToken(string token)
    {
        PlayerPrefs.SetString(SO.saveDataKeys.AccessTokenKey, token);
    }

    public void SetRefreshToken(string token)
    {
        PlayerPrefs.SetString(SO.saveDataKeys.RefreshTokenKey, token);
    }

    public string GetAccessToken()
    {
        return PlayerPrefs.GetString(SO.saveDataKeys.AccessTokenKey, string.Empty);
    }

    public string GetRefreshToken()
    {
        return PlayerPrefs.GetString(SO.saveDataKeys.RefreshTokenKey, string.Empty);
    }

    public void ClearTokens()
    {
        PlayerPrefs.DeleteKey(SO.saveDataKeys.AccessTokenKey);
        PlayerPrefs.DeleteKey(SO.saveDataKeys.RefreshTokenKey);
    }
    #endregion
}