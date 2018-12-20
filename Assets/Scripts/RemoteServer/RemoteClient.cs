using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkMessage;
using System;

public class RemoteClient : BaseClient
{
    private void Awake()
    {
        ServerDeltaTime = 1f / 30f;
    }

    public void AddObject(ClientObject clientObject)
    {
        objectDict.Add(clientObject.id, clientObject);
    }

    public void RemoveObject(long objectId)
    {
        Destroy(objectDict[objectId].gameObject);
        objectDict.Remove(objectId);
    }

    public void ReceiveSnapShot(SnapShot snapShot)
    {
        StopCoroutine("UpdateState");
        StartCoroutine("UpdateState", snapShot);
    }

    protected override void InputUpdate()
    {
        ProcessInput();
    }

    public override IEnumerator SendCommand(Command command)
    {
        ConnectionManager.Send(new Request(JsonUtility.ToJson(command), typeof(Command).Name.ToString()));
        yield return null;
    }
}
