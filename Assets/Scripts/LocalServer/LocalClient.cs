using System.Collections;
using System.Collections.Generic;
using NetworkMessage;
using UnityEngine;

public class LocalClient : BaseClient
{
    public float delayTime = 0.1f;
    public List<ClientObject> syncObjects;
    private LocalServer server;

    private void Awake()
    {
        server = FindObjectOfType<LocalServer>();
        ServerDeltaTime = 1f / server.tick;
        foreach (var o in syncObjects) objectDict.Add(o.id, o);
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
        StopCoroutine("UpdateState");
        StartCoroutine("UpdateState", snapShot);
        //StartCoroutine(UpdateState(snapShot));
    }

    public override IEnumerator SendCommand(Command command)
    {
        yield return new WaitForSeconds(delayTime);
        server.ReceiveCommand((int)objectIndex, command);
    }
}
