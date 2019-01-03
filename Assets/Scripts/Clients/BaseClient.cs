using System.Collections;
using System.Collections.Generic;
using NetworkMessage;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseClient : MonoBehaviour {
    public const int Tick = 50;
    protected const float time = 1.0f / Tick;
    public static float ServerDeltaTime;
    protected Dictionary<long, ClientObject> objectDict = new Dictionary<long, ClientObject>();
    protected Dictionary<long, Projectile> projectileDict = new Dictionary<long, Projectile>();
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

    private bool up;
    private bool down;
    private bool left;
    private bool right;
    private bool fire;

    private bool pressUp;
    private bool pressDown;
    private bool pressLeft;
    private bool pressRight;
    private bool pressFire;

    private const int fireRate = 1;
    private float timeNextFire;

    [SerializeField]
    private Button buttonA;
    [SerializeField]
    private Button buttonD;
    [SerializeField]
    private Button buttonW;
    [SerializeField]
    private Button buttonS;
    [SerializeField]
    private Button buttonSpace;
    
    public void ToggleDown(bool r) { pressDown = r; }
    public void ToggleUp(bool r) { pressUp = r; }
    public void ToggleLeft(bool r) { pressLeft = r; }
    public void ToggleRight(bool r) { pressRight = r; }
    public void ToggleFire(bool r) { pressFire = r; }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W) || pressUp)
        {
            up = true;
        }
        else if (Input.GetKey(KeyCode.S) || pressDown)
        {
            down = true;
        }
        if (Input.GetKey(KeyCode.D) || pressRight)
        {
            right = true;
        }
        else if (Input.GetKey(KeyCode.A) || pressLeft)
        {
            left = true;
        }
        if (Input.GetKey(KeyCode.Space) || pressFire)
        {
            if (timeNextFire < Time.timeSinceLevelLoad) {
                fire = true;
                timeNextFire = Time.timeSinceLevelLoad + 1f / fireRate;
            }
        }
    }

    private void FixedUpdate()
    {
        currentTick++;
        InputUpdate();
        foreach (var kv in objectDict)
        {
            kv.Value.GameUpdate(time);
        }
        foreach (var kv in projectileDict)
        {
            kv.Value.GameUpdate(time);
        }
    }

    protected void ProcessInput()
    {
        byte cmd = 0;
        if (up) cmd |= Command.Keys[KeyCode.W];
        else if (down) cmd |= Command.Keys[KeyCode.S];
        if (right) cmd |= Command.Keys[KeyCode.D];
        else if (left) cmd |= Command.Keys[KeyCode.A];
        if(fire) cmd |= Command.Keys[KeyCode.Space];

        SendCommand(cmd);
        up = false;down = false;right = false; left = false; fire = false;
    }

    protected IEnumerator UpdateState()
    {
        isProcessingShapShot = true;
        var snapShot = snapShots.Dequeue();
        var entities = snapShot.existingEntities;
        var newEntities = snapShot.newEntities;
        var moveEntities = snapShot.movingEntities;
        var destroyEntities = snapShot.destroyedEntities;

        for (int i = 0; i < destroyEntities.Count; i++)
        {
            Projectile obj = projectileDict[destroyEntities[i].id];
            Destroy(obj.gameObject);
            projectileDict.Remove(obj.id);
        }

        for (int i = 0; i < newEntities.Count; i++) {
            Projectile projectile = GameManager.Instance.CreateProjectile(newEntities[i]);
            projectileDict.Add(projectile.id, projectile);
        }

        for (int i = 0; i < moveEntities.Count; i++)
        {
            Projectile obj = projectileDict[moveEntities[i].id];
            var rot = Optimazation.DecompressRot(moveEntities[i].rotation);
            var pos = Optimazation.DecompressPos2(moveEntities[i].position);
            if (entityInterpolation)
            {
                obj.PrepareUpdate(pos);
            }
            else
            {
                //obj.transform.rotation = /*obj.transform.rotation =*/ rot;
                obj.transform.position = /*obj.transform.position = */ pos;
            }
        }

        for (int i = 0; i < entities.Count; i++)
        {
            long objId = entities[i].id;

            /*if (false && objectIndex == objId && reconcilation && prediction && cachedCmdNo == snapShot.commandId)
            {
                var rot = Optimazation.DecompressRot(entities[i].rotation);
                var pos = Optimazation.DecompressPos2(entities[i].position);
                var myObject = objectDict[objectIndex];
                myObject.desiredPosition += (pos - cachedPosition);
                myObject.desiredRotation = Quaternion.Euler(myObject.desiredRotation.eulerAngles + (rot.eulerAngles - cachedRotation));

            }*/

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
                    obj.PrepareUpdate(pos, rot);
                }
                else
                {
                    obj.desiredRotation = /*obj.transform.rotation =*/ rot;
                    obj.desiredPosition = /*obj.transform.position = */ pos;
                }
            }
        }
        if (entityInterpolation) yield return ServerDeltaTime;
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

    private void SendCommand(byte cmd)
    {
        commandSoFar++;
        var command = new Command(commandSoFar, currentTick, cmd);
        StartCoroutine(SendCommand(command));
        if (prediction)
        {
            objectDict[objectIndex].Predict(command);
            CachedTransform();
        }
    }

    private void CachedTransform()
    {
        /*if (commandSoFar % 5 == 0)
        {
            cachedCmdNo = commandSoFar;
            cachedPosition = objectDict[objectIndex].desiredPosition;
            cachedRotation = objectDict[objectIndex].desiredRotation.eulerAngles;
        }*/
    }
}
