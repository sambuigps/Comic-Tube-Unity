using UnityEngine;

public class GoogleButton : MonoBehaviour
{
    public void OnGoogleButton()
    {
        GoogleAuthHandler.Login(OnGoogleResponse);
    }

    private void OnGoogleResponse(ApiResponse<AuthResponse> response)
    {
        if (!response.success)
        {
            Debug.LogWarning(response.message);
            return;
        }
        else
        {
            Debug.Log("Username: " + response.data.user.username + " Email: " + response.data.user.email);
            Services.UI.SetUI(UI_Type.LogOut);
        }
    }
}
