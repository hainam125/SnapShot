using System;

public class Request {
    private static long currentId = 1;
    public long id;
    public string data;
    public string type;
    public Action<Response> callback;

    public Request(string dat, string datType, Action<Response> cb)
    {
        id = currentId++;
        callback = cb;
        data = dat;
        type = datType;
    }

    public Request(string dat, string datType)
    {
        id = -1;
        data = dat;
        type = datType;
    }
}
