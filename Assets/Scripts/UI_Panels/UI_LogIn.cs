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
    [SerializeField] Toggle showPass;
    public bool isInProgress = false;

    private void Awake()
    {
        emailOrUsernameField.AddComponent<InputFieldValidator>().ConfigureAll(ForceInput.Lowercase);
        passwordField.AddComponent<InputFieldValidator>().ConfigureAll(ForceInput.None, true, 20);
    }

    #region add and remove listeners
    private void OnEnable()
    {
        logInButton.onClick.AddListener(OnLogInButton);
        goToSignUpButton.onClick.AddListener(() => Services.UI.SetUI(UI_Type.SignUp));
        showPass.onValueChanged.AddListener((isOn) =>
        {
            passwordField.contentType = isOn ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
            passwordField.ForceLabelUpdate();
        });
    }

    private void OnDisable()
    {
        logInButton.onClick.RemoveAllListeners();
        showPass.onValueChanged.RemoveAllListeners();
        goToSignUpButton.onClick.RemoveAllListeners();
    }
    #endregion

    public override void Init()
    {
        base.Init();
        emailOrUsernameField.text = "";
        passwordField.text = "";
        showPass.isOn = false;
    }

    #region login
    private void OnLogInButton()
    {
        if (isInProgress)
        {
            Debug.LogWarning("Log in is already in progress");
            return;
        }
        isInProgress = true;
        if (!Validations.IsValidEmail(emailOrUsernameField.text) && !Validations.IsValidUsername(emailOrUsernameField.text))
        {
            Debug.LogWarning("Please enter a valid email/username");
            isInProgress = false;
            return;
        }
        if (!Validations.IsValidPassword(passwordField.text))
        {
            Debug.LogWarning("Password should be 8 to 20 characters long");
            isInProgress = false;
            return;
        }
        LoginHandler.Login(emailOrUsernameField.text, passwordField.text, OnLogInResponse);
    }

    private void OnLogInResponse(ApiResponse<AuthResponse> response)
    {
        isInProgress = false;

        if (!response.success)
        {
            Debug.LogWarning(response.message);
            return;
        }
        else
        {
            Debug.Log("Username: " + response.data.user.username + " Email: " + response.data.user.email);
            Services.UI.SetUI(UI_Type.Home);
        }
    }
    #endregion
}
