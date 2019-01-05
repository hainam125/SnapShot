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
    public Quaternion desiredRotation;
    public Vector3 desiredPosition;
    public int hp;

    private bool needUpdate;
    private float currentUpTime;
    private Vector3 startPos;
    private Vector3 targetPos;
    private Quaternion startRot;
    private Quaternion targetRot;

    [SerializeField]
    private Text nameTxt;
    [SerializeField]
    private RectTransform hpRect;
    [SerializeField]
    private GameObject mCamera;

    private void Awake()
    {
        desiredRotation = transform.rotation;
        desiredPosition = transform.position;
    }

    private void Update()
    {
        transform.rotation = desiredRotation;// Quaternion.Slerp(transform.rotation, desiredRotation, 0.5f);
        transform.position = desiredPosition;// Vector3.Lerp(transform.position, desiredPosition, 0.2f);
    }

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
            if (objects[i] != this && objects[i].isAlive() && transform.CheckCollision(objects[i].transform, desiredPosition, objects[i].desiredPosition))
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
        var objects = GameManager.Instance.playerObjects;
        var speed = ServerObject.Speed;
        Vector3 oldPos = desiredPosition;
        desiredPosition += speed * BaseClient.DeltaTime * transform.forward * direction;

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
            if (objects[i] != this && objects[i].isAlive() && transform.CheckCollision(objects[i].transform, desiredPosition, objects[i].desiredPosition))
            {
                desiredPosition = oldPos;
                return;
            }
        }
    }

    public void PrepareUpdate(Vector3 pos, Quaternion rot)
    {
        needUpdate = true;
        currentUpTime = 0f;
        targetPos = pos;
        targetRot = rot;
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
                desiredPosition = Vector3.Lerp(startPos, targetPos, currentUpTime / totalTime);
                desiredRotation = Quaternion.Slerp(startRot, targetRot, currentUpTime / totalTime);
            }
            else
            {
                desiredPosition = targetPos;
                desiredRotation = targetRot;
                needUpdate = false;
            }
        }
    }
}
