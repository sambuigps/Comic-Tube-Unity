using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_LogIn : UI_BaseClass
{
    [SerializeField] TMP_InputField emailOrUsernameField;
    [SerializeField] TMP_InputField passwordField;
    [SerializeField] Button logInButton;

    private void Awake()
    {
        emailOrUsernameField.AddComponent<InputFieldValidator>().typeAll(emailOrUsernameField, false, true, 20);
        passwordField.AddComponent<InputFieldValidator>().typeAll(passwordField, false, true, 20);
    }

    private void OnEnable()
    {
        logInButton.onClick.AddListener(() =>
            StartCoroutine(LoginHandler.Login(emailOrUsernameField.text, passwordField.text, (response) =>
            {
                Debug.Log("Username: " + response.data.user.username + " Email: " + response.data.user.email);
            })
        ));
    }

    private void OnDisable()
    {
        logInButton.onClick.RemoveAllListeners();
    }
}
