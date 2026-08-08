using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_LogIn : UI_BaseClass
{
    [SerializeField] TMP_InputField emailField;
    [SerializeField] TMP_InputField passwordField;
    [SerializeField] Button signInButton;

    private void Awake()
    {
        emailField.AddComponent<InputFieldValidator>().typeEmail(emailField, true, 50);
        passwordField.AddComponent<InputFieldValidator>().typeAll(passwordField, false, true, 20);
    }
}
