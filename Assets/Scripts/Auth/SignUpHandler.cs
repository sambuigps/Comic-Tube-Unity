using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class SignUpData
{
    public string platformType;
    public string username;
    public string email;
    public string password;

    public SignUpData(string email, string username, string password = "")
    {
        this.email = email;
        this.password = password;
        this.username = username;
        platformType = SO.env.platformType;
    }
}

public static class SignUpHandler
{
    public static IEnumerator Signup(string email, string username, string pass)
    {
        SignUpData body = new SignUpData(email, username, pass);

        string json = JsonUtility.ToJson(body);
        byte[] data = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(SO.env.SignupEndpoint, "POST");
        request.uploadHandler = new UploadHandlerRaw(data);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError(request.error);
        }
    }
}
