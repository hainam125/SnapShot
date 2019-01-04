using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkMessage;

public class RemoteClient : BaseClient
{
    private void Start()
    {
        ServerDeltaTime = 1f / 15f;
    }

    public void ReceiveSnapShot(SnapShot snapShot)
    {
        ProcessSnapShot(snapShot);
    }

    protected override void InputUpdate()
    {
        ProcessInput();
    }

    protected override void SendCommand(Command command)
    {
        ConnectionManager.Send(new Request(JsonUtility.ToJson(command), typeof(Command).Name.ToString()));
    }
}
