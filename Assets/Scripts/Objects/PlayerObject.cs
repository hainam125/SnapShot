using UnityEngine;
using NetworkMessage;

public class PlayerObject : MonoBehaviour
{
    private long id;
    public long Id { get { return id; } }
    public int currentHp;
    private int maxHp;
    public Vector3 RotateSpeed { get; private set; }
    public float MoveSpeed { get; private set; }

    private MoveTrajectory MoveTrajectory = new MoveTrajectory();
    private RotateTrajectory RotateTrajectory = new RotateTrajectory(Constant.ServerDeltaTime);

    private PlayerView view;

    private void Awake()
    {
        view = GetComponent<PlayerView>();
    }

    #region ===== Methods =====
    public void Init(Vector3 rotSpeed, float moveSpeed, int hp)
    {
        RotateSpeed = rotSpeed;
        MoveSpeed = moveSpeed;
        maxHp = hp;
    }

    public void SetId(long newId)
    {
        id = newId;
    }

    public void SetName(string name)
    {
        view.SetName(name);
    }

    public void ToggleCamera(bool active)
    {
        view.ToggleCamera(active);
    }

    public void SetHp()
    {
        SetHp(maxHp);
    }

    public void SetHp(int newHp)
    {
        if (newHp != currentHp)
        {
            if (currentHp <= 0 && newHp > 0) gameObject.SetActive(true);
            currentHp = newHp;
            view.UpdateHp(currentHp * 1f / maxHp);
            if (currentHp <= 0) gameObject.SetActive(false);
        }
    }

    public bool IsAlive()
    {
        return currentHp > 0;
    }

    public void UpdateState(Vector3 pos, Quaternion rot)
    {
        transform.rotation = rot;
        transform.position = pos;
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

        Quaternion oldRot = transform.rotation;
        transform.Rotate(RotateSpeed * deltaTime * direction);

        for (int i = 0; i < obstacles.Count; i++)
        {
            if (transform.CheckCollision(obstacles[i].transform, transform.position))
            {
                transform.rotation = oldRot;
                return;
            }
        }

        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] != this && objects[i].IsAlive() && transform.CheckCollision(objects[i].transform, transform.position, objects[i].transform.position))
            {
                transform.rotation = oldRot;
                return;
            }
        }
    }

    private void HandleMovement(float direction, float deltaTime)
    {
        var obstacles = GameManager.Instance.obstacles;
        var objects = GameManager.Instance.playerObjects;
        Vector3 oldPos = transform.position;
        transform.position += MoveSpeed * deltaTime * transform.forward * direction;
        for (int i = 0; i < obstacles.Count; i++)
        {
            if (transform.CheckCollision(obstacles[i].transform, transform.position))
            {
                transform.position = oldPos;
                return;
            }
        }
        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] != this && objects[i].IsAlive() && transform.CheckCollision(objects[i].transform, transform.position, objects[i].transform.position))
            {
                transform.position = oldPos;
                return;
            }
        }
    }

    #endregion

    #region ===== Interpolation =====
    public void PrepareUpdate(Vector3 pos, Quaternion rot)
    {   
        MoveTrajectory.Refresh(transform.position, pos, MoveSpeed);
        RotateTrajectory.Refresh(transform.rotation, rot);
    }

    public void GameUpdate(float deltaTime)
    {
        if (!MoveTrajectory.IsDone && !MoveTrajectory.CheckDone)
        {
            transform.position = MoveTrajectory.Update(deltaTime);
        }
        if(!RotateTrajectory.IsDone)
        {
            transform.rotation = RotateTrajectory.Update(deltaTime);
        }
    }
    #endregion
}
