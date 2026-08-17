using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_VerifyOtp : UI_BaseClass
{
    [SerializeField] TMP_InputField otpField;
    [SerializeField] Button verifyButton;
    [SerializeField] Button backButton;
    public bool isInProgress = false;

    private void Awake()
    {
        otpField.gameObject.AddComponent<InputFieldValidator>().ConfigureNumeric(true, 6);
    }

    private void OnEnable()
    {
        verifyButton.onClick.AddListener(OnVerifyButton);
        backButton.onClick.AddListener(OnBackButton);
    }

    private void OnDisable()
    {
        verifyButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
    }

    private void OnVerifyButton()
    {
        if (isInProgress)
        {
            Debug.LogWarning("Verifying...");
        }
        isInProgress = true;

        if(otpField.text.Length != 6)
        {
            Debug.LogWarning("Otp must be 6 digit");
            return;
        }
        VerifyOtpHandler.VerifyOtp(otpField.text, OnVerifyResponse);
    }

    private void OnVerifyResponse(ApiResponse<AuthResponse> response)
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

    private void OnBackButton()
    {
        Services.UI.SetUI(UI_Type.LogIn);
        Services.Session.unverifiedEmail = null;
    }
}
