using System.Collections;
using System.Collections.Generic;
using NetworkMessage;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseClient : MonoBehaviour {
    private const int Tick = 50;
    public const float DeltaTime = 1.0f / Tick;
    public static float ServerDeltaTime;
    protected Dictionary<long, PlayerObject> playObjectDict = new Dictionary<long, PlayerObject>();
    protected Dictionary<long, Projectile> projectileDict = new Dictionary<long, Projectile>();
    public long objectIndex;
    private bool prediction;
    private bool reconcilation;
    private bool entityInterpolation;
    private long commandSoFar = 0;
    private long currentTick;

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

    private const int fireRate = 3;
    private float timeNextFire;
    
    [SerializeField]
    private Toggle predictionToggle;
    [SerializeField]
    private Toggle reconcilationToggle;
    [SerializeField]
    private Toggle interpolationToggle;

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

    private bool pressUp;
    private bool pressDown;
    private bool pressLeft;
    private bool pressRight;
    private bool pressFire;

    public void ToggleDown(bool r) { pressDown = r; }
    public void ToggleUp(bool r) { pressUp = r; }
    public void ToggleLeft(bool r) { pressLeft = r; }
    public void ToggleRight(bool r) { pressRight = r; }
    public void ToggleUpRight(bool r) { pressUp = r; pressRight = r; }
    public void ToggleUpLeft(bool r) { pressUp = r; pressLeft = r; }
    public void ToggleDownRight(bool r) { pressDown = r; pressRight = r; }
    public void ToggleDownLeft(bool r) { pressDown = r; pressLeft = r; }
    public void ToggleFire(bool r) { pressFire = r; }

    private void Awake()
    {
        predictionToggle.onValueChanged.AddListener(value => prediction = value);
        reconcilationToggle.onValueChanged.AddListener(value => reconcilation = value);
        interpolationToggle.onValueChanged.AddListener(value => entityInterpolation = value);
    }

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
        foreach (var kv in playObjectDict)
        {
            kv.Value.GameUpdate(DeltaTime);
        }
        foreach (var kv in projectileDict)
        {
            kv.Value.GameUpdate(DeltaTime);
        }
    }

    public PlayerObject MainPlayer()
    {
        return playObjectDict[objectIndex];
    }

    protected void ProcessInput()
    {
        byte cmd = 0;
        if (up) cmd |= Command.Keys[KeyCode.W];
        else if (down) cmd |= Command.Keys[KeyCode.S];
        if (right) cmd |= Command.Keys[KeyCode.D];
        else if (left) cmd |= Command.Keys[KeyCode.A];
        if(fire) cmd |= Command.Keys[KeyCode.Space];

        if (cmd == 0) return;
        SendCommand(cmd);
        up = false; down = false; right = false; left = false; fire = false;
    }

    protected IEnumerator UpdateState()
    {
        isProcessingShapShot = true;
        var snapShot = snapShots.Dequeue();
        var entities = snapShot.existingEntities;
        var players = snapShot.existingPlayers;
        var newEntities = snapShot.newEntities;
        var destroyEntities = snapShot.destroyedEntities;

        for (int i = 0; i < destroyEntities.Count; i++)
        {
            RemoveProjectile(projectileDict[destroyEntities[i].id]);
        }

        for (int i = 0; i < newEntities.Count; i++) {
            AddProjectile(ObjectFactory.CreateProjectile(newEntities[i]));
        }

        for (int i = 0; i < entities.Count; i++)
        {
            Projectile obj = projectileDict[entities[i].id];
            var pos = Optimazation.DecompressPos2(entities[i].position);
            if (entityInterpolation)
            {
                obj.PrepareUpdate(pos);
            }
            else
            {
                obj.transform.position = pos;
            }
        }

        for (int i = 0; i < players.Count; i++)
        {
            long objId = players[i].id;
            var obj = playObjectDict[objId];

            obj.SetHp(players[i].hp);

            /*if (objectIndex == objId && reconcilation && prediction && cachedCmdNo == snapShot.commandId)
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
                var rot = Optimazation.DecompressRot(players[i].rotation);
                var pos = Optimazation.DecompressPos2(players[i].position);
                if (entityInterpolation && objectIndex != objId)
                {
                    obj.PrepareUpdate(pos, rot);
                }
                else
                {
                    obj.desiredRotation = rot;
                    obj.desiredPosition = pos;
                }
            }
        }

        if (entityInterpolation) yield return ServerDeltaTime;
        if (snapShots.Count > 0) StartCoroutine(UpdateState());
        else isProcessingShapShot = false;
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

    protected abstract void InputUpdate();

    protected abstract void SendCommand(Command command);

    private void SendCommand(byte cmd)
    {
        commandSoFar++;
        var command = new Command(commandSoFar, currentTick, cmd);
        SendCommand(command);
        if (prediction)
        {
            MainPlayer().Predict(command);
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

    public void AddObject(PlayerObject playerObject)
    {
        playObjectDict.Add(playerObject.id, playerObject);
    }

    public PlayerObject RemoveObject(long objectId)
    {
        var playerObject = playObjectDict[objectId];
        Destroy(playerObject.gameObject);
        playObjectDict.Remove(objectId);
        return playerObject;
    }

    public void AddProjectile(Projectile projectile)
    {
        projectileDict.Add(projectile.id, projectile);
    }

    public void RemoveProjectile(Projectile projectile)
    {
        Destroy(projectile.gameObject);
        projectileDict.Remove(projectile.id);
    }
}
