using System;
using System.Collections;
using System.Collections.Generic;
using NetworkMessage;
using UnityEngine;

public class LocalClient : MonoBehaviour
{
    public List<TestObject> syncObjects;
    public int objectIndex;
    public float delayTime = 0.1f;
    private LocalServer server;

    private void Awake()
    {
        server = FindObjectOfType<LocalServer>();
    }

    private void UpdateState(SnapShot snapShot)
    {
        var entities = snapShot.existingEntities;
        for (int i = 0; i < syncObjects.Count; i++)
        {
            syncObjects[i].setDesiredValue(entities[i]);
        }
    }

    public IEnumerator ReceiveSnapShot(SnapShot snapShot)
    {
        yield return new WaitForSeconds(delayTime);
        UpdateState(snapShot);
    }

    public IEnumerator SendCommand(Command command)
    {
        yield return new WaitForSeconds(delayTime);
        server.ReceiveCommand(objectIndex, command);
    }
}
