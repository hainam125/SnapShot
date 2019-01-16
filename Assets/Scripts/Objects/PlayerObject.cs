using UnityEngine;
using NetworkMessage;

public class PlayerObject : MonoBehaviour
{
    private Transform mTransform;
    public Vector3 Pos
    {
        get { return mTransform.position; }
        set { mTransform.position = value; }
    }
    public Quaternion Rot
    {
        get { return mTransform.rotation; }
        set { mTransform.rotation = value; }
    }
    public Vector3 PrePos { get; set; }

    private long id;
    public long Id { get { return id; } }
    public int currentHp;
    private int maxHp;

    public Vector3 RotateSpeed { get; private set; }
    public float MoveSpeed { get; private set; }

    private MoveTrajectory moveTrajectory;
    private RotateTrajectory rotateTrajectory;

    public PlayerView view;
    public PlayerSound sound;

    private StateMachine<PlayerObject> stateMachine;
    public StateMachine<PlayerObject> FSM { get { return stateMachine; } }

    private void Awake()
    {
        moveTrajectory = new MoveTrajectory();
        rotateTrajectory = new RotateTrajectory();
        mTransform = transform;
    }

    #region ===== Methods =====
    public void Init(Vector3 rotSpeed, float moveSpeed, int hp)
    {
        RotateSpeed = rotSpeed;
        MoveSpeed = moveSpeed;
        maxHp = hp;
        stateMachine = new StateMachine<PlayerObject>(this);
        stateMachine.currentState = PlayerState.Idling.Instance;
        stateMachine.currentState.Enter(this);
    }

    public bool IsAlive()
    {
        return currentHp > 0;
    }

    public void SetId(long newId)
    {
        id = newId;
    }

    public void SetHp()
    {
        SetHp(maxHp);
    }

    public void SetHp(int newHp)
    {
        if (newHp != currentHp)
        {
            currentHp = newHp;
            view.UpdateHp(currentHp * 1f / maxHp);
        }
    }

    public bool CheckRespawn(int newHp)
    {
        return currentHp <= 0 && newHp > 0;
    }

    public void UpdateState(Vector3 pos, Quaternion rot)
    {
        Rot = rot;
        Pos = pos;
    }

    #endregion

    #region ===== Prediction =====

    public void Predict(Command cmd, float deltaTime)
    {
        if(cmd.HasUp()) HandleMovement(1f, deltaTime);
        else if(cmd.HasDown()) HandleMovement(-1f, deltaTime);
        if (cmd.HasRight()) HandleRotation(1f, deltaTime);
        else if (cmd.HasLeft()) HandleRotation(-1f, deltaTime);
    }

    private void HandleRotation(float direction, float deltaTime)
    {
        var obstacles = GameManager.Instance.obstacles;
        var objects = GameManager.Instance.playerObjects;

        Quaternion oldRot = Rot;
        transform.Rotate(RotateSpeed * deltaTime * direction);

        for (int i = 0; i < obstacles.Count; i++)
        {
            if (transform.CheckCollision(obstacles[i].transform, Pos))
            {
                Rot = oldRot;
                return;
            }
        }

        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] != this && objects[i].IsAlive() && transform.CheckCollision(objects[i].transform, Pos, objects[i].Pos))
            {
                Rot = oldRot;
                return;
            }
        }
    }

    private void HandleMovement(float direction, float deltaTime)
    {
        var obstacles = GameManager.Instance.obstacles;
        var objects = GameManager.Instance.playerObjects;
        Vector3 oldPos = Pos;
        Pos += MoveSpeed * deltaTime * transform.forward * direction;
        for (int i = 0; i < obstacles.Count; i++)
        {
            if (transform.CheckCollision(obstacles[i].transform, Pos))
            {
                Pos = oldPos;
                return;
            }
        }
        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] != this && objects[i].IsAlive() && transform.CheckCollision(objects[i].transform, Pos, objects[i].Pos))
            {
                Pos = oldPos;
                return;
            }
        }
    }

    #endregion

    #region ===== Interpolation =====
    public void PrepareUpdate(Vector3 pos, Quaternion rot)
    {   
        moveTrajectory.Refresh(Pos, pos, MoveSpeed);
        rotateTrajectory.Refresh(Rot, rot);
    }

    public void GameUpdate(float deltaTime)
    {
        if (!moveTrajectory.IsDone && !moveTrajectory.CheckDone)
        {
            Pos = moveTrajectory.Update(deltaTime);
        }
        if(!rotateTrajectory.IsDone)
        {
            Rot = rotateTrajectory.Update(deltaTime);
        }
        stateMachine.Update();
    }
    #endregion
}
