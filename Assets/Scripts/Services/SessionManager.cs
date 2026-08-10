[System.Serializable]
public class SessionManager_Data : BaseDataClass
{
}

public class SessionManager : BaseService<SessionManager_Data>
{
    public User loggedInUser;

    public void CacheUser(User user, string accessToken, string refreshToken)
    {
        loggedInUser = user;
        Services.Save.SetAccessToken(accessToken);
        Services.Save.SetRefreshToken(refreshToken);
    }

    public void ClearUser()
    {
        loggedInUser = null;
        Services.Save.ClearTokens();
    }

    public string unverifiedEmail;
}