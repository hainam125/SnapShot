using System.Collections;
using System.Collections.Generic;
using NetworkMessage;
using UnityEngine;

public abstract class BaseClient : MonoBehaviour {
    public const int Tick = 30;
    protected const float time = 1.0f / Tick;
    public static float ServerDeltaTime;
    protected Dictionary<long, ClientObject> objectDict = new Dictionary<long, ClientObject>();
    public long objectIndex;
    public bool prediction;
    public bool reconcilation;
    public bool entityInterpolation;
    private long currentTick;
    protected long commandSoFar = 0;
    protected WaitForSeconds waitTime = new WaitForSeconds(time);

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
        for (int i = 0; i < entities.Count; i++)
        {
            long objId = entities[i].id;
            var obj = objectDict[objId];
            var rot = Optimazation.DecompressRot(entities[i].rotation);
            var pos = Optimazation.DecompressPos2(entities[i].position);
            if (entityInterpolation)
            {
                float currentTime = 0f;
                float maxTime = ServerDeltaTime;
                var cRot = obj.transform.rotation;
                var cPos = obj.transform.position;
                while (currentTime < maxTime)
                {
                    currentTime += time;
                    obj.desiredRotation = Quaternion.Slerp(cRot, rot, currentTime / ServerDeltaTime);
                    obj.desiredPosition = Vector3.Lerp(cPos, pos, currentTime / ServerDeltaTime);
                    yield return waitTime;
                }
                obj.desiredRotation = rot;
                obj.desiredPosition = pos;
            }
            else
            {
                obj.desiredRotation = rot;
                obj.desiredPosition = pos;
            }
        }
    }

    protected virtual void InputUpdate()
    {
        
    }

    protected void ProcessInput()
    {
        if (Input.GetKey(KeyCode.W)) SendUpCommand();
        else if (Input.GetKey(KeyCode.S)) SendDownCommand();
        if (Input.GetKey(KeyCode.D)) SendRightCommand();
        else if (Input.GetKey(KeyCode.A)) SendLeftCommand();
        if (Input.GetKey(KeyCode.Space)) SendRotateCommand();
    }

    public abstract IEnumerator SendCommand(Command command);

    private void SendUpCommand()
    {
        commandSoFar++;
        StartCoroutine(SendCommand(new Command(commandSoFar, currentTick, KeyCode.W)));
        if (prediction) objectDict[objectIndex].Predict(KeyCode.W);
    }

    private void SendDownCommand()
    {
        commandSoFar++;
        StartCoroutine(SendCommand(new Command(commandSoFar, currentTick, KeyCode.S)));
        if (prediction) objectDict[objectIndex].Predict(KeyCode.S);
    }

    private void SendLeftCommand()
    {
        commandSoFar++;
        StartCoroutine(SendCommand(new Command(commandSoFar, currentTick, KeyCode.A)));
        if (prediction) objectDict[objectIndex].Predict(KeyCode.A);
    }

    private void SendRightCommand()
    {
        commandSoFar++;
        StartCoroutine(SendCommand(new Command(commandSoFar, currentTick, KeyCode.D)));
        if (prediction) objectDict[objectIndex].Predict(KeyCode.D);
    }

    private void SendRotateCommand()
    {
        commandSoFar++;
        StartCoroutine(SendCommand(new Command(commandSoFar, currentTick, KeyCode.Space)));
        if (prediction) objectDict[objectIndex].Predict(KeyCode.Space);
    }
}
