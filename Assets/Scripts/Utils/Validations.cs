public static class Validations
{
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email)) return false;
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidUsername(string username)
    {
        if (string.IsNullOrEmpty(username)) return false;
        if (username.Length < 3 || username.Length > 20) return false;
        foreach (char c in username)
        {
            if( char.IsUpper(c) ||
                char.IsLower(c) ||
                char.IsDigit(c) ||
                c == '_' || c == '-') continue;
            return false;
        }
        return true;
    }

    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrEmpty(password)) return false;
        if (password.Length < 8) return false;
        bool hasUpper = false, hasLower = false, hasDigit = false, hasSpecial = false;
        foreach (char c in password)
        {
            if (c > 127) return false; // no unicodes
            if (char.IsUpper(c)) hasUpper = true;
            else if (char.IsLower(c)) hasLower = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else hasSpecial = true;
        }
        return hasUpper && hasLower && hasDigit && hasSpecial;
    }
}
