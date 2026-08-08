using UnityEngine;
using UnityEngine.UI;

public class UI_LogOut : UI_BaseClass
{
    [SerializeField] Button logOutButton;

    private void OnEnable()
    {
        logOutButton.onClick.AddListener(OnLogOutButton);
    }

    private void OnDisable()
    {
        logOutButton.onClick.RemoveAllListeners();
    }

    private void OnLogOutButton()
    {
        LogOutHandler.LogOut(OnLogOutResponse);
    }

    private void OnLogOutResponse(ApiResponse<LogOutResponse> response)
    {
        Debug.Log(response.message);
        Services.UI.SetUI(UI_Type.SignUp);
    }
}
