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
        //Debug.Log(desiredPosition);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, 0.5f);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, 0.2f);
    }

    public void SetName(string name)
    {
        nameTxt.transform.parent.gameObject.SetActive(true);
        nameTxt.text = name;
    }

    public void Predict(KeyCode code)
    {
        var speed = ServerObject.Speed;
        switch (code) {
            case KeyCode.W:
                desiredPosition += speed * deltaTime * transform.forward;
                break;
            case KeyCode.S:
                desiredPosition -= speed * deltaTime * transform.forward;
                break;
            case KeyCode.D:
                transform.Rotate(ServerObject.RotateSpeed * deltaTime);
                desiredRotation = transform.rotation;
                break;
            case KeyCode.A:
                transform.Rotate(-ServerObject.RotateSpeed * deltaTime);
                desiredRotation = transform.rotation;
                break;
        }
    }
}
