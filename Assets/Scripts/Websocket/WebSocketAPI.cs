using System;
using System.Collections;
using UnityEngine;
using UniRx;
using WebSocketSharp;

public class WebSocketAPI : MonoBehaviour
{
    private WebSocket ws;

    private Action OnConnect = delegate { };
    private Action OnDisconnect = delegate { };
    private Action<string> OnGetMessage = delegate { };

    public bool SendMsg(string msg)
    {
        if (ws.IsAlive)
        {
            ws.Send(msg);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetCallback(Action<string> onGetMessage, Action onConnect, Action onDisconnect)
    {
        OnGetMessage = onGetMessage;
        OnConnect = onConnect;
        OnDisconnect = onDisconnect;
    }

    public void Init(string url)
    {
        ws = new WebSocket(url);
        ws.ConnectAsync();
        Connect();
    }

    public void Connect()
    {
        var hotWsObservable = new WebSocketObservable(ws, OnConnect)
          .Create()
          .ObserveOn(Scheduler.MainThread).Publish();
        hotWsObservable.Connect();

        hotWsObservable
          .Subscribe(
            x =>
            {
                OnGetMessage(x);
            },
            ex =>
            {
                Debug.Log(ex.Message);
                OnDisconnect();
                StartCoroutine(Reconnect());
            },
            () =>
            {
                Debug.Log("Cannot connect to host");
                StartCoroutine(Reconnect());
            })
          .AddTo(gameObject);

    }

    private IEnumerator Reconnect()
    {
        yield return new WaitForSeconds(5);
        Connect();
    }

    private void OnDestroy()
    {
        if (ws != null && ws.IsAlive) ws.Close();
    }
}
