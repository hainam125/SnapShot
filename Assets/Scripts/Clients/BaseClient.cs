using System.Collections;
using System.Collections.Generic;
using NetworkMessage;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseClient : MonoBehaviour {
    private const int Tick = 50;
    public const float DeltaTime = 1.0f / Tick;
    protected Dictionary<long, PlayerObject> playObjectDict = new Dictionary<long, PlayerObject>();
    protected Dictionary<long, Projectile> projectileDict = new Dictionary<long, Projectile>();
    public long objectIndex;
    private bool prediction;
    private bool reconcilation;
    private bool entityInterpolation;
    private long commandSoFar = 0;
    private long currentTick;

    protected Queue<SnapShot> snapShots = new Queue<SnapShot>();
    protected bool isProcessingShapShot;

    [SerializeField]
    private ClientInput input;

    [SerializeField]
    private Toggle predictionToggle;
    [SerializeField]
    private Toggle reconcilationToggle;
    [SerializeField]
    private Toggle interpolationToggle;
    
    private void Awake()
    {
        predictionToggle.onValueChanged.AddListener(value => prediction = value);
        reconcilationToggle.onValueChanged.AddListener(value => reconcilation = value);
        interpolationToggle.onValueChanged.AddListener(value => entityInterpolation = value);
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
                    obj.transform.rotation = rot;
                    obj.transform.position = pos;
                }
            }
        }

        if (entityInterpolation) yield return Constant.ServerDeltaTime;
        if (snapShots.Count > 0) StartCoroutine(UpdateState());
        else isProcessingShapShot = false;
    }
    
    protected void ProcessSnapShot(SnapShot snapShot)
    {
        snapShots.Enqueue(snapShot);
        if (!isProcessingShapShot)
        {
            StartCoroutine(UpdateState());
        }
    }

    protected virtual void InputUpdate()
    {
        var cmd = input.InputUpdate();
        if(cmd != 0) SendCommand(cmd);
    }

    protected abstract void SendCommand(Command command);

    private void SendCommand(byte cmd)
    {
        commandSoFar++;
        var command = new Command(commandSoFar, currentTick, cmd);
        SendCommand(command);
        if (prediction)
        {
            MainPlayer().Predict(command);
        }
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
        projectileDict.Add(projectile.Id, projectile);
    }

    public void RemoveProjectile(Projectile projectile)
    {
        Destroy(projectile.gameObject);
        projectileDict.Remove(projectile.Id);
    }
}
