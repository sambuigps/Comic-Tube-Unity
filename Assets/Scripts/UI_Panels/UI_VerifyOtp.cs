using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_VerifyOtp : UI_BaseClass
{
    [SerializeField] TMP_InputField otpField;
    [SerializeField] Button verifyButton;
    public bool isInProgress = false;

    private void Awake()
    {
        otpField.AddComponent<InputFieldValidator>().ConfigureNumeric(true, 6);
    }

    private void OnEnable()
    {
        verifyButton.onClick.AddListener(OnVerifyButton);
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

    private void OnVerifyResponse(ApiResponse<VerifyOtpResponse> response)
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
