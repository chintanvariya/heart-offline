using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace HeartCardGame
{
    public static class HT_APIManager
    {
        // Send data in header and body is null
        public static IEnumerator RequestWithGetData(string url, Action<string> CallBackResponse = null, Action<string> CallbackError = null)
        {
            Debug.Log("<color=yellow> URL => " + url + "</color>");
            HT_GameManager.instance.uiManager.reconnectionPanel.SetActive(true);
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // request.SetRequestHeader("Authorization", ProfileEdit.instance.token);
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(request.error);
                    CallbackError?.Invoke(request.error);
                }
                else
                {
                    Debug.Log(request.downloadHandler.text);
                    CallBackResponse?.Invoke(request.downloadHandler.text);
                }
                HT_GameManager.instance.uiManager.reconnectionPanel.SetActive(false);
            }
        }

        // Send data in body
        public static IEnumerator RequestWithPostData(string url, string postData, Action<string> CallBackResponse = null, Action<string> CallbackError = null)
        {
            WWWForm form = new WWWForm();
            form.AddField("data", postData);

            Debug.Log($"<color=white> API Url <====> {url} \n {postData} </color>");
            HT_GameManager.instance.uiManager.reconnectionPanel.SetActive(true);
            using (UnityWebRequest request = UnityWebRequest.Post(url, form))
            {
                if (url.Contains("runninggame"))
                    request.SetRequestHeader("deviceid", SystemInfo.deviceUniqueIdentifier);
                else
                    request.SetRequestHeader("authorization", PlayerPrefs.GetString("Token"));
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(request.error);
                    CallbackError?.Invoke(request.error);
                }
                else
                {
                    Debug.Log(request.downloadHandler.text);
                    CallBackResponse?.Invoke(request.downloadHandler.text);
                }
                yield return new WaitForSeconds(1f);
                HT_GameManager.instance.uiManager.reconnectionPanel.SetActive(false);
            }
        }
    }

    public static class Network
    {
        public static IEnumerator InterConnectionCheck(Action<bool> isNetwork)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                isNetwork?.Invoke(false);
            }
            else
            {
                using (UnityWebRequest request = UnityWebRequest.Head("https://www.google.com/"))
                {
                    yield return request.SendWebRequest();
                    if (request.result == UnityWebRequest.Result.ConnectionError)
                    {
                        isNetwork?.Invoke(false);
                    }
                    else
                    {
                        isNetwork?.Invoke(true);
                    }
                }
            }
        }
    }
}