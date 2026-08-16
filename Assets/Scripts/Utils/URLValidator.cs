using System;

public static class URLValidator
{
    public static bool IsValid(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) 
            return false;

        return Uri.TryCreate(url, UriKind.Absolute, out Uri uri)
               && (uri.Scheme == Uri.UriSchemeHttp 
               || uri.Scheme == Uri.UriSchemeHttps);
    }
}
