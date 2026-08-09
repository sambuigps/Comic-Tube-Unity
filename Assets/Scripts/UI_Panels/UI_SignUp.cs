using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_SignUp : UI_BaseClass
{
    [SerializeField] TMP_InputField emailField;
    [SerializeField] TMP_InputField usernameField;
    [SerializeField] TMP_InputField passwordField;
    [SerializeField] Button signUpButton;
    [SerializeField] Button goToLogInButton;
    [SerializeField] Toggle showPass;
    public bool isInProgress = false;

    private void Awake()
    {
        emailField.AddComponent<InputFieldValidator>().ConfigureEmail();
        usernameField.AddComponent<InputFieldValidator>().ConfigureAlphanumeric(ForceInput.Lowercase, true, 20, true);
        passwordField.AddComponent<InputFieldValidator>().ConfigureAll(ForceInput.None, true, 20);
    }

    private void OnEnable()
    {
        signUpButton.onClick.AddListener(OnSignUpButton);
        goToLogInButton.onClick.AddListener(() => Services.UI.SetUI(UI_Type.LogIn));
        showPass.onValueChanged.AddListener((isOn) =>
        {
            passwordField.contentType = isOn ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
            passwordField.ForceLabelUpdate();
        });
    }

    private void OnDisable()
    {
        signUpButton.onClick.RemoveAllListeners();
        showPass.onValueChanged.RemoveAllListeners();
    }

    public override void Init()
    {
        base.Init();
        emailField.text = "";
        usernameField.text = "";
        passwordField.text = "";
        showPass.isOn = false;
    }

    private void OnSignUpButton()
    {
        if (isInProgress)
        {
            Debug.LogWarning("Sign up is already in progress");
            return;
        }
        isInProgress = true;
        if (!Validations.IsValidEmail(emailField.text)) 
        {
            Debug.LogWarning("Invalid email");
            isInProgress = false;
            return;
        }
        if (!Validations.IsValidUsername(usernameField.text))
        {
            Debug.LogWarning("Username should be 3 to 20 characters long");
            isInProgress = false;
            return;
        }
        if (!Validations.IsValidPassword(passwordField.text))
        {
            Debug.LogWarning("Password should be 8 to 20 characters long");
            isInProgress = false;
            return;
        }
        SignUpHandler.Signup(emailField.text, usernameField.text, passwordField.text, OnSignUpResponse);
    }

    private void OnSignUpResponse(ApiResponse<SignUpResponse> response)
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
            Services.UI.SetUI(UI_Type.LogOut);
        }
    }
}
