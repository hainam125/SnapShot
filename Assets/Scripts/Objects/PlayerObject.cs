﻿using UnityEngine;
using UnityEngine.UI;
using NetworkMessage;

public class PlayerObject : MonoBehaviour
{
    public long id;
    public int currentHp;
    private int maxHp;
    public Vector3 RotateSpeed { get; private set; }
    public float MoveSpeed { get; private set; }

    private MoveTrajectory MoveTrajectory = new MoveTrajectory();
    private RotateTrajectory RotateTrajectory = new RotateTrajectory(Constant.ServerDeltaTime);

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
            hpRect.localScale = new Vector3(currentHp * 1f / maxHp, 1f, 1f);
            if (currentHp <= 0) gameObject.SetActive(false);
        }
    }

    public void Init(Vector3 rotSpeed, float moveSpeed, int hp)
    {
        RotateSpeed = rotSpeed;
        MoveSpeed = moveSpeed;
        maxHp = hp;
    }

    public bool isAlive()
    {
        return currentHp > 0;
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
        transform.Rotate(RotateSpeed * BaseClient.DeltaTime * direction);

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
        Vector3 oldPos = transform.position;
        transform.position += MoveSpeed * BaseClient.DeltaTime * transform.forward * direction;
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
}
