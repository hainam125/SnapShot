using System;
using System.Collections;
using System.Collections.Generic;
using NetworkMessage;
using UnityEngine;

public class LocalClient : MonoBehaviour
{
    public List<ClientObject> syncObjects;
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
            syncObjects[i].SetDesiredValue(entities[i]);
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


    private void Update()
    {
        if (objectIndex == 0)
        {
            if (Input.GetKey(KeyCode.W)) SendUpCommand();
            else if (Input.GetKey(KeyCode.S)) SendDownCommand();
            if (Input.GetKey(KeyCode.D)) SendRightCommand();
            else if (Input.GetKey(KeyCode.A)) SendLeftCommand();
            if (Input.GetKey(KeyCode.Space)) SendRotateCommand();
        }
    }

    private void SendUpCommand()
    {
        StartCoroutine(SendCommand(new Command(KeyCode.W)));
    }

    private void SendDownCommand()
    {
        StartCoroutine(SendCommand(new Command(KeyCode.S)));
    }

    private void SendLeftCommand()
    {
        StartCoroutine(SendCommand(new Command(KeyCode.A)));
    }

    private void SendRightCommand()
    {
        StartCoroutine(SendCommand(new Command(KeyCode.D)));
    }

    private void SendRotateCommand()
    {
        StartCoroutine(SendCommand(new Command(KeyCode.Space)));
    }
}
