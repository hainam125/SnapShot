using System;
using System.Collections;
using System.Collections.Generic;
using NetworkMessage;
using UnityEngine;

public class LocalClient : MonoBehaviour
{
    public const int Tick = 40;
    public List<ClientObject> syncObjects;
    public int objectIndex;
    public float delayTime = 0.1f;
    public bool prediction;
    private LocalServer server;
    private long commandSoFar = 0;
    private bool pressUp;
    private bool pressDown;
    private bool pressRight;
    private bool pressLeft;
    private bool pressRot;

    private void Awake()
    {
        server = FindObjectOfType<LocalServer>();
    }

    private IEnumerator Start()
    {
        var time = 1.0f / Tick;
        while (true)
        {
            InputUpdate();
            yield return new WaitForSeconds(time);
        }
    }

    private void UpdateState(SnapShot snapShot)
    {
        if (prediction && snapShot.commandId < commandSoFar) return;
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

    private void InputUpdate()
    {
        /*if(objectIndex == 0)
        {
            Debug.Log("=====================");
        }*/
        if (pressUp)
        {
            SendUpCommand();
            pressUp = false;
        }
        if (pressDown)
        {
            SendDownCommand();
            pressDown = false;
        }
        if (pressLeft)
        {
            SendLeftCommand();
            pressLeft = false;
        }
        if (pressRight)
        {
            SendRightCommand();
            pressRight = false;
        }
        if (pressRot)
        {
            SendRotateCommand();
            pressRot = false;
        }
    }


    private void Update()
    {
        if (objectIndex == 0)
        {
            if (Input.GetKey(KeyCode.W)) pressUp = true;
            else if (Input.GetKey(KeyCode.S)) pressDown = true;
            if (Input.GetKey(KeyCode.D)) pressRight = true;
            else if (Input.GetKey(KeyCode.A)) pressLeft = true;
            if (Input.GetKey(KeyCode.Space)) pressRot = true;
        }
    }

    private void SendUpCommand()
    {
        commandSoFar++;
        StartCoroutine(SendCommand(new Command(commandSoFar, KeyCode.W)));
        if (prediction) syncObjects[objectIndex].Predict(KeyCode.W);
    }

    private void SendDownCommand()
    {
        commandSoFar++;
        StartCoroutine(SendCommand(new Command(commandSoFar, KeyCode.S)));
        if (prediction) syncObjects[objectIndex].Predict(KeyCode.S);
    }

    private void SendLeftCommand()
    {
        commandSoFar++;
        StartCoroutine(SendCommand(new Command(commandSoFar, KeyCode.A)));
        if(prediction) syncObjects[objectIndex].Predict(KeyCode.A);
    }

    private void SendRightCommand()
    {
        commandSoFar++;
        StartCoroutine(SendCommand(new Command(commandSoFar, KeyCode.D)));
        if (prediction) syncObjects[objectIndex].Predict(KeyCode.D);
    }

    private void SendRotateCommand()
    {
        commandSoFar++;
        StartCoroutine(SendCommand(new Command(commandSoFar, KeyCode.Space)));
        if (prediction) syncObjects[objectIndex].Predict(KeyCode.Space);
    }
}
