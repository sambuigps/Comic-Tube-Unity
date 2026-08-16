using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class UrlImageHandler
{
    public static void LoadImage(string url, Action<Sprite> callback = null)
    {
        if (!URLValidator.IsValid(url))
        {
            callback?.Invoke(null);
            return;
        }
        Services.Instance.StartCoroutine(GetImage(url, callback));
    }

    public static IEnumerator GetImage(string url, Action<Sprite> callback)
    {
        using UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
        var operation = req.SendWebRequest();

        yield return operation;

        if(req.result != UnityWebRequest.Result.Success)
        {
            callback?.Invoke(null);
            yield break;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(req);

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );

        callback?.Invoke(sprite);
    }
}
