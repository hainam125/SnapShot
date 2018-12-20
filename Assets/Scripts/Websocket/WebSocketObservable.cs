using System;
using System.Threading;
using UnityEngine;
using UniRx;
using WebSocketSharp;

public class WebSocketObservable
{
    private static double DisconnectTimeCheck = 5;
    private static int DisconnectCountCheck = 3;

    private WebSocket ws;
    private Action onConnection;
    private Thread checkConnectionThread;

    public WebSocketObservable(WebSocket _ws, Action _onConnection)
    {
        ws = _ws;
        onConnection = _onConnection;
    }
    
    public IObservable<string> Create()
    {
        return Observable.Create<string>(observer =>
        {
            var compositeDisposable = new CompositeDisposable();
            var subject = new Subject<string>();
            compositeDisposable.Add(subject.Subscribe(observer));
            compositeDisposable.Add(Message().Subscribe(subject));
            compositeDisposable.Add(Check().Subscribe(subject));
            return compositeDisposable;
        });
    }

    public IObservable<string> Message()
    {
        var observale = Observable.Create<string>(observer =>
        {
            EventHandler<MessageEventArgs> received = (sender, e) => { observer.OnNext(e.Data); };

            EventHandler<CloseEventArgs> closed = (sender, e) => { observer.OnCompleted(); };

            EventHandler<ErrorEventArgs> error = (sender, e) => { observer.OnError(e.Exception); };

            EventHandler opened = (sender, e) => { onConnection(); };

            ws.OnMessage += received;
            ws.OnClose += closed;
            ws.OnError += error;
            ws.OnOpen += opened;

            return Disposable.Create(() =>
            {
                ws.OnMessage -= received;
                ws.OnClose -= closed;
                ws.OnError -= error;
                ws.OnOpen -= opened;
                ws.CloseAsync();
            });
        });
        return observale;
    }

    public IObservable<string> Check()
    {
        var scheduler = Scheduler.ThreadPool;
        return Observable.Create<string>(observer =>
        {
            var cancellation = new CancellationDisposable();
            var scheduleWork = scheduler.Schedule(() =>
            {
                try
                {
                    int count = 0;
                    while (!ws.IsAlive)
                    {
                        count++;
                        Thread.Sleep(TimeSpan.FromSeconds(DisconnectTimeCheck));
                        if (count >= DisconnectCountCheck)
                        {
                            observer.OnCompleted();
                            break;
                        }
                    }
                    while (true)
                    {
                        cancellation.Token.ThrowIfCancellationRequested();
                        Thread.Sleep(TimeSpan.FromSeconds(DisconnectTimeCheck));
                        if (ws.IsAlive)
                        {
                            //Debug.Log("connected");
                        }
                        else
                        {
                            Debug.Log("not connected");
                            observer.OnError(new Exception("Lost Connect"));
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    observer.OnError(e);
                }
            });
            return new CompositeDisposable(scheduleWork, cancellation);
        });
    }
}
