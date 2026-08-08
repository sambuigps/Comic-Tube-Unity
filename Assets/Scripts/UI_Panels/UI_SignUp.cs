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
        emailField.AddComponent<InputFieldValidator>().typeEmail(emailField, true, 20);
        usernameField.AddComponent<InputFieldValidator>().typeAlphanumeric(usernameField, false, true, 20, true);
        passwordField.AddComponent<InputFieldValidator>().typeAll(passwordField, false, true, 20);
    }

    private void OnEnable()
    {
        signUpButton.onClick.AddListener(() =>
            StartCoroutine(SignUpHandler.Signup(emailField.text, usernameField.text, passwordField.text, (response) =>
            {
            })
        ));
    }

    private void OnDisable()
    {
        signUpButton.onClick.RemoveAllListeners();
    }
}
