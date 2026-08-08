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
            var result = new ApiResponse<TResponse>();
            result.success = false;

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                // request never reached the server
                Debug.LogError($"{request.responseCode}: {request.error}");
                result.errorType = ApiErrorType.Network;
                result.message = "Something went wrong!";
            }
            else // request reached the server
            {
                result.errorType = ApiErrorType.Api;
                try
                {
                    var errorResponse = JsonConvert.DeserializeObject<ApiResponse<TResponse>>(request.downloadHandler.text);
                    result.message = errorResponse.message;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to parse error body: {e.Message}\n{request.downloadHandler.text}");
                    result.message = "Something went wrong!";
                }
            }

            callback?.Invoke(result);
            request.Dispose();
            yield break;
        }

        var response = JsonConvert.DeserializeObject<ApiResponse<TResponse>>(request.downloadHandler.text);
        callback?.Invoke(new ApiResponse<TResponse> { success = true, data = response.data, message = response.message });
        request.Dispose();
    }
}