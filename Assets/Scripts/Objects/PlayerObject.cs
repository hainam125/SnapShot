using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetworkMessage;

public class PlayerObject : MonoBehaviour
{
    private const int MaxHp = 5;
    public const int PrefabId = 0;
    public long id;
    public int hp;

    [SerializeField]
    private Text nameTxt;
    [SerializeField]
    private RectTransform hpRect;
    [SerializeField]
    private GameObject mCamera;

    public void SetName(string name)
    {
        nameTxt.transform.parent.gameObject.SetActive(true);
        nameTxt.text = name;
    }

    public void SetHp(int newHp = MaxHp)
    {
        if (newHp != hp)
        {
            if (hp <= 0 && newHp > 0) gameObject.SetActive(true);
            hp = newHp;
            hpRect.localScale = new Vector3(hp * 1f / MaxHp, 1f, 1f);
            if (hp <= 0) gameObject.SetActive(false);
        }
    }

    public bool isAlive()
    {
        return hp > 0;
    }

    public void ToggleCamera(bool active)
    {
        mCamera.SetActive(active);
    }

    public void Predict(Command cmd)
    {
        if(cmd.HasUp()) HandleMovement(1f);
        else if(cmd.HasDown()) HandleMovement(-1f);
        if (cmd.HasRight()) HandleRotation(1f);
        else if (cmd.HasLeft()) HandleRotation(-1f);
    }

    private void HandleRotation(float direction)
    {
        var obstacles = GameManager.Instance.obstacles;
        var objects = GameManager.Instance.playerObjects;

        Quaternion oldRot = transform.rotation;
        transform.Rotate(ServerObject.RotateSpeed * BaseClient.DeltaTime * direction);

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
            if (objects[i] != this && objects[i].isAlive() && transform.CheckCollision(objects[i].transform, transform.position, objects[i].transform.position))
            {
                transform.rotation = oldRot;
                return;
            }
        }
    }

    private void HandleMovement(float direction)
    {
        var obstacles = GameManager.Instance.obstacles;
        var objects = GameManager.Instance.playerObjects;
        var speed = ServerObject.Speed;
        Vector3 oldPos = transform.position;
        transform.position += speed * BaseClient.DeltaTime * transform.forward * direction;
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
            if (objects[i] != this && objects[i].isAlive() && transform.CheckCollision(objects[i].transform, transform.position, objects[i].transform.position))
            {
                transform.position = oldPos;
                return;
            }
        }
    }

    public void PrepareUpdate(Vector3 pos, Quaternion rot)
    {   
        MoveTrajectory.Refresh(transform.position, pos, ServerObject.Speed);
        RotateTrajectory.Refresh(transform.rotation, rot);
    }
    protected MoveTrajectory MoveTrajectory = new MoveTrajectory();
    protected RotateTrajectory RotateTrajectory = new RotateTrajectory();

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
}
