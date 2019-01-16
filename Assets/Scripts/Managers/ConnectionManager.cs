using System;
using System.Collections.Generic;
using UnityEngine;
using NetworkMessage;

public class ConnectionManager : MonoBehaviour
{
    private static ConnectionManager instance;
    private WebSocketAPI api;
    private Queue<Request> requests = new Queue<Request>();
    private Dictionary<long, Request> waitingRequests = new Dictionary<long, Request>();

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (requests.Count > 0)
        {
            var request = requests.Dequeue();
            api.SendMsg(JsonUtility.ToJson(request));
            if (request.id > 0) waitingRequests.Add(request.id, request);
        }
    }

    public static void Connect(string url, Action onSuccess, Action onFail)
    {
        instance.ConnectTo(url, onSuccess, onFail);
    }

    public static void Send(Request request)
    {
        instance.requests.Enqueue(request);
    }

    private void ConnectTo(string url, Action onSuccess, Action onFail)
    {
        api = new GameObject().AddComponent<WebSocketAPI>();
        api.SetCallback(str =>
        {
            ProcessResponse(str);
        }, () =>
        {
            onSuccess();
        }, () =>
        {
            onFail();
        });
        api.Init(url);
    }

    private void ProcessResponse(string data)
    {
        //Debug.Log(data);
        var response = JsonUtility.FromJson<Response>(data);
        if(response.id == -1)
        {
            if(response.type == typeof(UserJoined).Name.ToString())
            {
                GameManager.Instance.Join(response);
            }
            else if (response.type == typeof(UserExited).Name.ToString())
            {
                GameManager.Instance.Exit(response);
            }
            else if (response.type == typeof(SnapShot).Name.ToString())
            {
                GameManager.Instance.ReceiveSnapShot(response);
            }
        }
        else if (waitingRequests.ContainsKey(response.id))
        {
            var request = waitingRequests[response.id];
            request.callback.Invoke(response);
            waitingRequests.Remove(response.id);
        }
    }
}