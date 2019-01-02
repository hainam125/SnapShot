using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetworkMessage;

public class ClientObject : MonoBehaviour
{
    public const int PrefabId = 0;
    public long id;
    private static float deltaTime = 1f / LocalClient.Tick;
    public Quaternion desiredRotation;
    public Vector3 desiredPosition;
    [SerializeField]
    private Text nameTxt;

    private void Awake () {
        desiredRotation = transform.rotation;
        desiredPosition = transform.position;
    }
	
	private void Update () {
        transform.rotation = desiredRotation;// Quaternion.Slerp(transform.rotation, desiredRotation, 0.5f);
        transform.position = desiredPosition;// Vector3.Lerp(transform.position, desiredPosition, 0.2f);
    }

    public void SetName(string name)
    {
        nameTxt.transform.parent.gameObject.SetActive(true);
        nameTxt.text = name;
    }

    public void Predict(KeyCode code)
    {
        switch (code)
        {
            case KeyCode.W:
                HandleMovement(1f);
                break;
            case KeyCode.S:
                HandleMovement(-1f);
                break;
            case KeyCode.D:
                HandleRotation(1f);
                break;
            case KeyCode.A:
                HandleRotation(-1f);
                break;
        }
    }

    private void HandleRotation(float direction)
    {
        var obstacles = GameManager.Instance.obstacles;
        var objects = GameManager.Instance.objects;

        Quaternion oldRot = transform.rotation;
        transform.Rotate(ServerObject.RotateSpeed * deltaTime * direction);
        desiredRotation = transform.rotation;

        for (int i = 0; i < obstacles.Count; i++)
        {
            if (transform.CheckCollision(obstacles[i].transform, desiredPosition))
            {
                transform.rotation = oldRot;
                desiredRotation = oldRot;
                return;
            }
        }
        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] != this && transform.CheckCollision(objects[i].transform, desiredPosition, objects[i].desiredPosition))
            {
                transform.rotation = oldRot;
                desiredRotation = oldRot;
                return;
            }
        }
    }

    private void HandleMovement(float direction)
    {
        var obstacles = GameManager.Instance.obstacles;
        var objects = GameManager.Instance.objects;
        var speed = ServerObject.Speed;
        Vector3 oldPos = desiredPosition;
        desiredPosition += speed * deltaTime * transform.forward * direction;

        for (int i = 0; i < obstacles.Count; i++)
        {
            if (transform.CheckCollision(obstacles[i].transform, desiredPosition))
            {
                desiredPosition = oldPos;
                return;
            }
        }
        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] != this && transform.CheckCollision(objects[i].transform, desiredPosition, objects[i].desiredPosition))
            {
                desiredPosition = oldPos;
                return;
            }
        }

    }

    private bool needUpdate;
    private float currentUpTime;
    private Vector3 startPos;
    private Vector3 targetPos;
    private Quaternion startRot;
    private Quaternion targetRot;

    public void PrepareUpdate(Vector3 pos, Quaternion rot)
    {
        needUpdate = true;
        currentUpTime = 0f;
        targetPos = pos;
        startPos = transform.position;
        startRot = transform.rotation;
    }

    public void GameUpdate(float deltaTime)
    {
        if (!needUpdate) return;
        var totalTime = BaseClient.ServerDeltaTime;
        if (currentUpTime < totalTime)
        {
            currentUpTime += deltaTime;
            var nextTime = currentUpTime + deltaTime;
            if (nextTime < totalTime)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, currentUpTime / totalTime);
                transform.rotation = Quaternion.Slerp(startRot, targetRot, currentUpTime / totalTime);
            }
            else
            {
                transform.position = targetPos;
                transform.rotation = targetRot;
                needUpdate = false;
            }
        }
    }
}
