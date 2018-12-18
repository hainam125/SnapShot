using System;
using System.Collections;
using System.Collections.Generic;
using NetworkMessage;
using UnityEngine;

public class LocalClient : MonoBehaviour
{
    public const int Tick = 30;
    private const float time = 1.0f / Tick;
    public static float ServerDeltaTime;
    public List<ClientObject> syncObjects;
    public int objectIndex;
    public float delayTime = 0.1f;
    public bool prediction;
    public bool reconcilation;
    public bool entityInterpolation;
    private LocalServer server;
    private long currentTick;
    private long commandSoFar = 0;
    private WaitForSeconds waitTime = new WaitForSeconds(time);

    private void Awake()
    {
        server = FindObjectOfType<LocalServer>();
        ServerDeltaTime = 1f / server.tick;
    }

    private IEnumerator Start()
    {
        while (true)
        {
            currentTick++;
            InputUpdate();
            yield return waitTime;
        }
    }

    private IEnumerator UpdateState(SnapShot snapShot)
    {
        if (reconcilation && prediction && snapShot.commandId < commandSoFar) yield break;
        var entities = snapShot.existingEntities;
        for (int i = 0; i < syncObjects.Count; i++)
        {
            if (entities[i] == null) continue;
            var rot = Optimazation.DecompressRot(entities[i].rotation);
            var pos = Optimazation.DecompressPos1(entities[i].position);
            if (entityInterpolation)
            {
                float currentTime = 0f;
                float maxTime = ServerDeltaTime;
                var cRot = syncObjects[i].transform.rotation;
                var cPos = syncObjects[i].transform.position;
                while (currentTime < maxTime)
                {
                    currentTime += time;
                    syncObjects[i].desiredRotation = Quaternion.Slerp(cRot, rot, currentTime / ServerDeltaTime);
                    syncObjects[i].desiredPosition = Vector3.Lerp(cPos, pos, currentTime / ServerDeltaTime);
                    yield return waitTime;
                }
                syncObjects[i].desiredRotation = rot;
                syncObjects[i].desiredPosition = pos;
            }
            else
            {
                syncObjects[i].desiredRotation = rot;
                syncObjects[i].desiredPosition = pos;
            }
        }
    }

    public IEnumerator ReceiveSnapShot(SnapShot snapShot)
    {
        yield return new WaitForSeconds(delayTime);
        StopCoroutine("UpdateState");
        StartCoroutine("UpdateState", snapShot);
        //StartCoroutine(UpdateState(snapShot));
    }

    public IEnumerator SendCommand(Command command)
    {
        yield return new WaitForSeconds(delayTime);
        server.ReceiveCommand(objectIndex, command);
    }

    private void InputUpdate()
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
        commandSoFar++;
        StartCoroutine(SendCommand(new Command(commandSoFar, currentTick, KeyCode.W)));
        if (prediction) syncObjects[objectIndex].Predict(KeyCode.W);
    }

    private void SendDownCommand()
    {
        commandSoFar++;
        StartCoroutine(SendCommand(new Command(commandSoFar, currentTick, KeyCode.S)));
        if (prediction) syncObjects[objectIndex].Predict(KeyCode.S);
    }

    private void SendLeftCommand()
    {
        commandSoFar++;
        StartCoroutine(SendCommand(new Command(commandSoFar, currentTick, KeyCode.A)));
        if(prediction) syncObjects[objectIndex].Predict(KeyCode.A);
    }

    private void SendRightCommand()
    {
        commandSoFar++;
        StartCoroutine(SendCommand(new Command(commandSoFar, currentTick, KeyCode.D)));
        if (prediction) syncObjects[objectIndex].Predict(KeyCode.D);
    }

    private void SendRotateCommand()
    {
        commandSoFar++;
        StartCoroutine(SendCommand(new Command(commandSoFar, currentTick, KeyCode.Space)));
        if (prediction) syncObjects[objectIndex].Predict(KeyCode.Space);
    }
}
