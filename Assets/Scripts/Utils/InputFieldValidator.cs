using System.Text;
using TMPro;
using UnityEngine;

public class InputFieldValidator : MonoBehaviour
{
    public enum InputType
    {
        Alphanumeric,
        Numeric,
        All,
        Email
    }

    private bool allowUnderscores;

    [Header("References")]
    [SerializeField] TMP_InputField inputField;

    [Header("Input")]
    [SerializeField] InputType inputType = InputType.Alphanumeric;

    [Header("Length")]
    [SerializeField] bool restrictLength = true;
    [SerializeField] int maxLength = 12;

    [Header("Numeric")]
    [SerializeField] bool clampValue = false;
    [SerializeField] int minValue = 0;
    [SerializeField] int maxValue = 9999;

    [Header("Formatting")]
    [SerializeField] bool forceUppercase = true;
    [SerializeField] bool insertSeparator = false;
    [SerializeField] char separatorChar = '-';
    [SerializeField] int separatorInterval = 4;

    private bool updating;

    private void OnEnable()
    {
        if (inputField != null)
            inputField.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnDisable()
    {
        if (inputField != null)
            inputField.onValueChanged.RemoveListener(OnValueChanged);
    }

    #region inits
    public void typeNumeric(
    TMP_InputField inputField,
    bool clampValue = false,
    int minValue = 0,
    int maxValue = 9999)
    {
        if (this.inputField != null)
            this.inputField.onValueChanged.RemoveListener(OnValueChanged);

        this.inputField = inputField;

        this.inputField.onValueChanged.RemoveListener(OnValueChanged);
        this.inputField.onValueChanged.AddListener(OnValueChanged);

        inputType = InputType.Numeric;

        this.clampValue = clampValue;
        this.minValue = minValue;
        this.maxValue = maxValue;

        forceUppercase = false;
        insertSeparator = false;
    }

    public void typeAlphanumeric(
    TMP_InputField inputField,
    bool forceUppercase = true,
    bool restrictLength = true,
    int maxLength = 12,
    bool allowUndercores = false,
    bool insertSeparator = false,
    char separatorChar = '-',
    int separatorInterval = 4)
    {
        if (this.inputField != null)
            this.inputField.onValueChanged.RemoveListener(OnValueChanged);

        this.allowUnderscores = allowUndercores;

        this.inputField = inputField;

        this.inputField.onValueChanged.RemoveListener(OnValueChanged);
        this.inputField.onValueChanged.AddListener(OnValueChanged);

        inputType = InputType.Alphanumeric;

        this.forceUppercase = forceUppercase;

        this.restrictLength = restrictLength;
        this.maxLength = maxLength;

        this.insertSeparator = insertSeparator;
        this.separatorChar = separatorChar;
        this.separatorInterval = separatorInterval;

        clampValue = false;
    }

    public void typeEmail(
    TMP_InputField inputField,
    bool restrictLength = true,
    int maxLength  = 50)
    {
        if (this.inputField != null)
            this.inputField.onValueChanged.RemoveListener(OnValueChanged);

        this.inputField = inputField;

        this.inputField.onValueChanged.RemoveListener(OnValueChanged);
        this.inputField.onValueChanged.AddListener(OnValueChanged);

        inputType = InputType.Email;

        this.restrictLength = restrictLength;
        this.maxLength = maxLength;

        forceUppercase = false;
        insertSeparator = false;
        clampValue = false;
    }

    public void typeAll(
    TMP_InputField inputField,
    bool forceUppercase = false,
    bool restrictLength = true,
    int maxLength = 12,
    bool insertSeparator = false,
    char separatorChar = '-',
    int separatorInterval = 4)
    {
        if (this.inputField != null)
            this.inputField.onValueChanged.RemoveListener(OnValueChanged);

        this.inputField = inputField;

        this.inputField.onValueChanged.RemoveListener(OnValueChanged);
        this.inputField.onValueChanged.AddListener(OnValueChanged);

        inputType = InputType.All;

        this.forceUppercase = forceUppercase;

        this.restrictLength = restrictLength;
        this.maxLength = maxLength;

        this.insertSeparator = insertSeparator;
        this.separatorChar = separatorChar;
        this.separatorInterval = separatorInterval;

        clampValue = false;
    }
    #endregion

    private void OnValueChanged(string value)
    {
        if (updating) return;

        updating = true;

        string result = inputType switch
        {
            InputType.Numeric => ProcessNumeric(value),
            InputType.Alphanumeric => ProcessAlphanumeric(value),
            InputType.Email => ProcessEmail(value),
            InputType.All => ProcessAll(value),
            _ => ProcessAll(value)
        };

        if (result != value)
        {
            inputField.SetTextWithoutNotify(result);

            Canvas.ForceUpdateCanvases();
            inputField.MoveTextEnd(false);
        }

        updating = false;
    }

    private string ProcessAll(string value)
    {
        StringBuilder sb = new();

        foreach (char c in value)
        {
            char finalChar = forceUppercase ? char.ToUpper(c) : c;

            sb.Append(finalChar);

            if (restrictLength && sb.Length >= maxLength)
                break;
        }

        return ApplySeparator(sb.ToString());
    }

    private string ProcessEmail(string value)
    {
        StringBuilder sb = new();
        bool hasAt = false;

        foreach (char c in value)
        {
            if (char.IsLetterOrDigit(c) ||
                c == '.' ||
                c == '_' ||
                c == '-' ||
                c == '+')
            {
                sb.Append(c);
            }
            else if (c == '@' && !hasAt)
            {
                hasAt = true;
                sb.Append(c);
            }

            if (restrictLength && sb.Length >= maxLength)
                break;
        }

        return sb.ToString();
    }

    private string ProcessAlphanumeric(string value)
    {
        StringBuilder sb = new();

        foreach (char c in value)
        {
            if (!char.IsLetterOrDigit(c) &&
            !(allowUnderscores && (c == '_' || c == '-')))
                continue;

            char finalChar = forceUppercase ? char.ToUpper(c) : c;

            sb.Append(finalChar);

            if (restrictLength && sb.Length >= maxLength)
                break;
        }

        return ApplySeparator(sb.ToString());
    }

    private string ProcessNumeric(string value)
    {
        StringBuilder sb = new();

        foreach (char c in value)
        {
            if (!char.IsDigit(c))
                continue;

            sb.Append(c);

            if (restrictLength && sb.Length >= maxLength)
                break;
        }

        string result = sb.ToString();

        if (clampValue && result.Length > 0 && int.TryParse(result, out int number))
        {
            number = Mathf.Clamp(number, minValue, maxValue);
            result = number.ToString();
        }

        return result;
    }

    private string ApplySeparator(string value)
    {
        if (!insertSeparator || separatorInterval <= 0)
            return value;

        StringBuilder sb = new();

        for (int i = 0; i < value.Length; i++)
        {
            if (i > 0 && i % separatorInterval == 0)
                sb.Append(separatorChar);

            sb.Append(value[i]);
        }

        return sb.ToString();
    }
}