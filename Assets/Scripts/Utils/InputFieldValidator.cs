using System.Text;
using TMPro;
using UnityEngine;

public enum ForceInput
{
    Uppercase,
    Lowercase,
    None
}

[RequireComponent(typeof(TMP_InputField))]
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
    TMP_InputField inputField;

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
    [SerializeField] ForceInput forceInput = ForceInput.None;

    private bool updating;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onValueChanged.AddListener(OnValueChanged);
    }

    #region inits
    public void ConfigureNumeric(
    bool clampValue = false,
    int minValue = 0,
    int maxValue = 9999)
    {
        inputType = InputType.Numeric;

        this.clampValue = clampValue;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public void ConfigureAlphanumeric(
    ForceInput forceInput,
    bool restrictLength = false,
    int maxLength = 12,
    bool allowUndercores = false)
    {
        this.allowUnderscores = allowUndercores;

        inputType = InputType.Alphanumeric;

        this.forceInput = forceInput;

        this.restrictLength = restrictLength;
        this.maxLength = maxLength;
    }

    public void ConfigureEmail()
    {
        inputType = InputType.Email;
    }

    public void ConfigureAll(
    ForceInput forceInput,
    bool restrictLength = false,
    int maxLength = 12)
    {
        inputType = InputType.All;

        this.forceInput = forceInput;

        this.restrictLength = restrictLength;
        this.maxLength = maxLength;
    }
    #endregion

    private void OnValueChanged(string value)
    {
        if (updating) return;

        updating = true;

        int oldCaretPosition = inputField.caretPosition;

        string preprocessed = Preprocess(value);

        string result = inputType switch
        {
            InputType.Numeric => ProcessNumeric(preprocessed),
            InputType.Alphanumeric => ProcessAlphanumeric(preprocessed),
            InputType.Email => ProcessEmail(preprocessed),
            InputType.All => ProcessAll(preprocessed),
            _ => ProcessAll(preprocessed)
        };

        if (result != value)
        {
            string beforeCaret = value[..Mathf.Min(oldCaretPosition, value.Length)];

            string processedBeforeCaret = inputType switch
            {
                InputType.Numeric => ProcessNumeric(Preprocess(beforeCaret)),
                InputType.Alphanumeric => ProcessAlphanumeric(Preprocess(beforeCaret)),
                InputType.Email => ProcessEmail(Preprocess(beforeCaret)),
                InputType.All => ProcessAll(Preprocess(beforeCaret)),
                _ => ProcessAll(Preprocess(beforeCaret))
            };

            int newCaretPosition = Mathf.Min(
                processedBeforeCaret.Length,
                result.Length
            );

            inputField.SetTextWithoutNotify(result);

            inputField.caretPosition = newCaretPosition;
            inputField.selectionAnchorPosition = newCaretPosition;
            inputField.selectionFocusPosition = newCaretPosition;
        }

        updating = false;
    }

    private static bool IsAllowedCharacter(char c)
    {
        return char.IsUpper(c) ||
           char.IsLower(c) ||
           char.IsDigit(c) ||
           "!@#$%^&*(),.?\":{}|<>_-+=/\\[];'`~".Contains(c);
    }

    private static string Preprocess(string value)
    {
        StringBuilder sb = new(value.Length);

        foreach (char c in value)
        {
            if (IsAllowedCharacter(c))
                sb.Append(c);
        }

        return sb.ToString();
    }

    private string ProcessAll(string value)
    {
        StringBuilder sb = new();

        foreach (char c in value)
        {
            char finalChar;
            if (forceInput == ForceInput.Uppercase) finalChar = char.ToUpper(c);
            else if (forceInput == ForceInput.Lowercase) finalChar = char.ToLower(c);
            else finalChar = c;

            sb.Append(finalChar);

            if (restrictLength && sb.Length >= maxLength)
                break;
        }

        return sb.ToString();
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
                sb.Append(char.ToLowerInvariant(c));
            }
            else if (c == '@' && !hasAt)
            {
                hasAt = true;
                sb.Append(c);
            }
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

            char finalChar;
            if (forceInput == ForceInput.Uppercase) finalChar = char.ToUpper(c);
            else if (forceInput == ForceInput.Lowercase) finalChar = char.ToLower(c);
            else finalChar = c;

            sb.Append(finalChar);

            if (restrictLength && sb.Length >= maxLength)
                break;
        }

        return sb.ToString();
    }

    private string ProcessNumeric(string value)
    {
        StringBuilder sb = new();

        foreach (char c in value)
        {
            if (!char.IsDigit(c))
                continue;

            sb.Append(c);
        }

        string result = sb.ToString();

        if (clampValue && result.Length > 0 && int.TryParse(result, out int number))
        {
            number = Mathf.Clamp(number, minValue, maxValue);
            result = number.ToString();
        }

        return result;
    }
}