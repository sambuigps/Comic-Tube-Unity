using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_LogIn : UI_BaseClass
{
    [SerializeField] TMP_InputField emailOrUsernameField;
    [SerializeField] TMP_InputField passwordField;
    [SerializeField] Button logInButton;
    [SerializeField] Button goToSignUpButton;

    private void Awake()
    {
        emailOrUsernameField.AddComponent<InputFieldValidator>().ConfigureAll(ForceInput.Lowercase);
        passwordField.AddComponent<InputFieldValidator>().ConfigureAll(ForceInput.None, true, 20);
    }

    private void OnEnable()
    {
        logInButton.onClick.AddListener(OnLogInButton);
        goToSignUpButton.onClick.AddListener(() => Services.UI.SetUI(UI_Type.SignUp));
    }

    private void OnDisable()
    {
        logInButton.onClick.RemoveAllListeners();
    }

    private void OnLogInButton()
    {
        if (!Validations.IsValidEmail(emailOrUsernameField.text) && !Validations.IsValidUsername(emailOrUsernameField.text))
        {
            Debug.LogWarning("Please enter a valid email/username");
            return;
        }
        if (!Validations.IsValidPassword(passwordField.text))
        {
            Debug.LogWarning("Password should be 8 to 20 characters long");
            return;
        }
        LoginHandler.Login(emailOrUsernameField.text, passwordField.text, OnLogInResponse);
    }

    private void OnLogInResponse(ApiResponse<LoginResponse> response)
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
