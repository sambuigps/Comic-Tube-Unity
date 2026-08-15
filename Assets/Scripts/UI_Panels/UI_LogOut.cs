using UnityEngine;
using UnityEngine.UI;

public class UI_LogOut : UI_BaseClass
{
    [SerializeField] Button logOutButton;
    public bool isInProgress = false;

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
        if (isInProgress) return;
        isInProgress = true;
        LogOutHandler.LogOut(OnLogOutResponse);
    }

    private void OnLogOutResponse(ApiResponse<object> response)
    {
        isInProgress = false;

        Debug.Log(response.message);
        Services.UI.SetUI(UI_Type.SignUp);
    }
}
