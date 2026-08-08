using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class ApiHandler
{
    public static void Send<TRequest, TResponse>(
        string url,
        string method,
        TRequest body,
        Action<ApiResponse<TResponse>> callback)
    {
        Services.Instance.StartCoroutine(SendRoutine(url, method, body, callback));
    }

    public static IEnumerator SendRoutine<TRequest, TResponse>(
        string url,
        string method,
        TRequest body,
        Action<ApiResponse<TResponse>> callback)
    {
        var request = new UnityWebRequest(url, method);

        if (body != null)
        {
            string json = JsonConvert.SerializeObject(body);
            request.uploadHandler =
                new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));

            request.SetRequestHeader("Content-Type", "application/json");
        }

        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{request.responseCode}: {request.error}");
            callback?.Invoke(null);
            yield break;
        }

        var response = JsonConvert.DeserializeObject<ApiResponse<TResponse>>(request.downloadHandler.text);

        callback?.Invoke(response);

        request.Dispose();
    }
}