using System.Collections;
using System.Collections.Generic;
using NetworkMessage;
using UnityEngine;

public abstract class BaseClient : MonoBehaviour
{
    private float deltaTime;
    protected Dictionary<long, PlayerObject> playObjectDict = new Dictionary<long, PlayerObject>();
    protected Dictionary<long, Projectile> projectileDict = new Dictionary<long, Projectile>();
    public long objectIndex;
    private bool isPrediction = true;
    private bool isReconcilation = true;
    private bool isInterpolation = true;
    private long commandSoFar = 0;
    private long currentTick;

    protected Queue<SnapShot> snapShots = new Queue<SnapShot>();
    protected bool isProcessingShapShot;
    
    private ClientInput input;

    #region ===== Main methods =====
    private void Awake()
    {
        input = GetComponent<ClientInput>();
        deltaTime = Time.fixedDeltaTime;
        Debug.Log(deltaTime);
    }

    private void FixedUpdate()
    {
        currentTick++;
        UpdateInput();
        foreach (var kv in playObjectDict)
        {
            kv.Value.GameUpdate(deltaTime);
        }
        foreach (var kv in projectileDict)
        {
            kv.Value.GameUpdate(deltaTime);
        }
    }

    #endregion

    #region ===== Properties =====

    public void SetPrediction(bool value)
    {
        isPrediction = value;
    }

    public void SetReconcilation(bool value)
    {
        isReconcilation = value;
    }

    public void SetInterpolation(bool value)
    {
        isInterpolation = value;
    }

    #endregion

    #region ===== Snapshot =====

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

            if (isInterpolation) obj.PrepareUpdate(pos);
            else obj.UpdateState(pos);
        }

        for (int i = 0; i < players.Count; i++)
        {
            long objId = players[i].id;
            var obj = playObjectDict[objId];

            var newHp = players[i].hp;

            bool hasRespawn = obj.CheckRespawn(newHp);
            obj.SetHp(newHp);

            if (objectIndex == objId && isReconcilation && isPrediction && snapShot.commandId < commandSoFar)
            {
                isProcessingShapShot = false;
                continue;
            }
            else
            {

                var rot = Optimazation.DecompressRot(players[i].rotation);
                var pos = Optimazation.DecompressPos2(players[i].position);

                if (isInterpolation && objectIndex != objId && !hasRespawn) obj.PrepareUpdate(pos, rot); 
                else obj.UpdateState(pos, rot);
            }
        }

        //yield return Constant.ServerDeltaTime;
        yield return null;

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

    #endregion

    #region ===== Send Input =====

    protected virtual void UpdateInput()
    {
        var cmd = input.GetCmd();
        if(cmd != 0 && GetMainPlayer().IsAlive()) SendCommand(cmd);
    }

    private void SendCommand(byte cmd)
    {
        commandSoFar++;
        var command = new Command(commandSoFar, currentTick, cmd);
        SendCommand(command);
        if (isPrediction)
        {
            GetMainPlayer().Predict(command, deltaTime);
        }
    }

    protected abstract void SendCommand(Command command);

    #endregion

    #region ===== Objects =====

    public PlayerObject GetMainPlayer()
    {
        return playObjectDict[objectIndex];
    }

    public void AddObject(PlayerObject playerObject)
    {
        playObjectDict.Add(playerObject.Id, playerObject);
    }

    public void AddMainObject(PlayerObject playerObject)
    {
        AddObject(playerObject);
        input.UpdateFireRate(playerObject.FireRate);
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
        projectileDict.Remove(projectile.Id);
        projectile.ShowExplode();
    }
    #endregion
}
