using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetworkMessage;

public class ClientObject : MonoBehaviour
{
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
                //transform.position += speed * deltaTime * Vector3.forward;
                //desiredPosition = transform.position;
                desiredPosition += speed * deltaTime * Vector3.forward;
                break;
            case KeyCode.S:
                //transform.position -= speed * deltaTime * Vector3.forward;
                //desiredPosition = transform.position;
                desiredPosition -= speed * deltaTime * Vector3.forward;
                break;
            case KeyCode.D:
                //transform.position += speed * deltaTime * Vector3.right;
                //desiredPosition = transform.position;
                desiredPosition += speed * deltaTime * Vector3.right;
                break;
            case KeyCode.A:
                //transform.position -= speed * deltaTime * Vector3.right;
                //desiredPosition = transform.position;
                desiredPosition -= speed * deltaTime * Vector3.right;
                break;
            case KeyCode.Space:
                transform.Rotate(ServerObject.RotateSpeed * deltaTime);
                desiredRotation = transform.rotation;
                break;
        }
    }
}
