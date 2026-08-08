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

    private void Awake()
    {
        emailField.AddComponent<InputFieldValidator>().ConfigureEmail();
        usernameField.AddComponent<InputFieldValidator>().ConfigureAlphanumeric(ForceInput.Lowercase, true, 20, true);
        passwordField.AddComponent<InputFieldValidator>().ConfigureAll(ForceInput.None, true, 20);
    }

    private void OnEnable()
    {
        signUpButton.onClick.AddListener(OnSignUpButton);
    }

    private void OnDisable()
    {
        signUpButton.onClick.RemoveAllListeners();
    }

    private void OnSignUpButton()
    {
        if (!Validations.IsValidEmail(emailField.text)) 
        {
            Debug.LogWarning("Invalid email");
            return;
        }
        if (!Validations.IsValidUsername(usernameField.text))
        {
            Debug.LogWarning("Username should be 3 to 20 characters long");
            return;
        }
        if (!Validations.IsValidPassword(passwordField.text))
        {
            Debug.LogWarning("Password should be 8 to 20 characters long");
            return;
        }
        SignUpHandler.Signup(emailField.text, usernameField.text, passwordField.text, OnSignUpResponse);
    }

    private void OnSignUpResponse(ApiResponse<SignUpResponse> response)
    {

    }
}
