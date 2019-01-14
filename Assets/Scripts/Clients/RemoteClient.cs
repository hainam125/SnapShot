using UnityEngine;
using NetworkMessage;

public class RemoteClient : BaseClient
{
    public void ReceiveSnapShot(SnapShot snapShot)
    {
        ProcessSnapShot(snapShot);
    }

    protected override void SendCommand(Command command)
    {
        ConnectionManager.Send(new Request(JsonUtility.ToJson(command), typeof(Command).Name.ToString()));
    }
}
