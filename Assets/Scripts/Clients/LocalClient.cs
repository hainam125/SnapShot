using System.Collections;
using System.Collections.Generic;
using NetworkMessage;
using UnityEngine;

public class LocalClient : BaseClient
{
    public float delayTime = 0.1f;
    public List<PlayerObject> playerObjects;
    private LocalServer server;

    private void Start()
    {
        server = FindObjectOfType<LocalServer>();
        ServerDeltaTime = 1f / server.tick;
        foreach (var o in playerObjects) playObjectDict.Add(o.id, o);
    }

    protected override void InputUpdate()
    {
        if (objectIndex == 1)
        {
            ProcessInput();
        }
    }

    public IEnumerator ReceiveSnapShot(SnapShot snapShot)
    {
        yield return new WaitForSeconds(delayTime);
        ProcessSnapShot(snapShot);
    }

    protected override void SendCommand(Command command)
    {
        StartCoroutine(SendCommandWithDelay(command));
    }

    private IEnumerator SendCommandWithDelay(Command command)
    {
        yield return new WaitForSeconds(delayTime);
        server.ReceiveCommand((int)objectIndex, command);
    }
}
