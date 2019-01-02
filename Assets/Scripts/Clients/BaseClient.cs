using System.Collections;
using System.Collections.Generic;
using NetworkMessage;
using UnityEngine;

public abstract class BaseClient : MonoBehaviour {
    public const int Tick = 50;
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

    protected Queue<SnapShot> snapShots = new Queue<SnapShot>();
    protected bool hasStartedProcessingSnapShot;
    protected bool isProcessingShapShot;
    private long cachedCmdNo;
    private Vector3 cachedPosition;
    private Vector3 cachedRotation;

    bool up;
    bool down;
    bool left;
    bool right;

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            up = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            down = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            right = true;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            left = true;
        }
    }

    private void FixedUpdate()
    {
        currentTick++;
        InputUpdate();
    }

    protected void ProcessInput()
    {
        if (up)
        {
            up = false;
            SendUpCommand();
        }
        else if (down)
        {
            down = false;
            SendDownCommand();
        }
        if (right)
        {
            right = false;
            SendRightCommand();
        }
        else if (left)
        {
            left = false;
            SendLeftCommand();
        }
    }

    private IEnumerator Start1()
    {
        while (true)
        {
            currentTick++;
            InputUpdate();
            yield return waitTime;
        }
    }

    protected IEnumerator UpdateState()
    {
        isProcessingShapShot = true;
        var snapShot = snapShots.Dequeue();
        var entities = snapShot.existingEntities;
        for (int i = 0; i < entities.Count; i++)
        {
            long objId = entities[i].id;

            if (false && objectIndex == objId && reconcilation && prediction && cachedCmdNo == snapShot.commandId)
            {
                var rot = Optimazation.DecompressRot(entities[i].rotation);
                var pos = Optimazation.DecompressPos2(entities[i].position);
                var myObject = objectDict[objectIndex];
                myObject.desiredPosition += (pos - cachedPosition);
                myObject.desiredRotation = Quaternion.Euler(myObject.desiredRotation.eulerAngles + (rot.eulerAngles - cachedRotation));

            }

            if (objectIndex == objId && reconcilation && prediction && snapShot.commandId < commandSoFar)
            {
                isProcessingShapShot = false;
                continue;
            }
            else
            {
                var obj = objectDict[objId];
                var rot = Optimazation.DecompressRot(entities[i].rotation);
                var pos = Optimazation.DecompressPos2(entities[i].position);
                if (entityInterpolation && objectIndex != objId)
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
                    obj.desiredRotation = /*obj.transform.rotation =*/ rot;
                    obj.desiredPosition = /*obj.transform.position = */ pos;
                }
            }
        }
        if (snapShots.Count > 0) StartCoroutine(UpdateState());
        else isProcessingShapShot = false;
    }

    protected virtual void InputUpdate()
    {
    }

    protected void ProcessSnapShot(SnapShot snapShot)
    {
        snapShots.Enqueue(snapShot);
        if (!hasStartedProcessingSnapShot && snapShots.Count > 1)
        {
            hasStartedProcessingSnapShot = true;
            StartCoroutine(UpdateState());
        }
        else if (!isProcessingShapShot)
        {
            StartCoroutine(UpdateState());
        }
    }
    
    public abstract IEnumerator SendCommand(Command command);

    private void SendUpCommand()
    {
        commandSoFar++;
        StartCoroutine(SendCommand(new Command(commandSoFar, currentTick, KeyCode.W)));
        if (prediction)
        {
            objectDict[objectIndex].Predict(KeyCode.W);
            CachedTransform();
        }
    }

    private void SendDownCommand()
    {
        commandSoFar++;
        StartCoroutine(SendCommand(new Command(commandSoFar, currentTick, KeyCode.S)));
        if (prediction)
        {
            objectDict[objectIndex].Predict(KeyCode.S);
            CachedTransform();
        }
    }

    private void SendLeftCommand()
    {
        commandSoFar++;
        StartCoroutine(SendCommand(new Command(commandSoFar, currentTick, KeyCode.A)));
        if (prediction)
        {
            objectDict[objectIndex].Predict(KeyCode.A);
            CachedTransform();
        }
    }

    private void SendRightCommand()
    {
        commandSoFar++;
        StartCoroutine(SendCommand(new Command(commandSoFar, currentTick, KeyCode.D)));
        if (prediction)
        {
            objectDict[objectIndex].Predict(KeyCode.D);
            CachedTransform();
        }
    }

    private void CachedTransform()
    {
        if (commandSoFar % 5 == 0)
        {
            cachedCmdNo = commandSoFar;
            cachedPosition = objectDict[objectIndex].desiredPosition;
            cachedRotation = objectDict[objectIndex].desiredRotation.eulerAngles;
        }
    }
}
