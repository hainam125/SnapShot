using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkMessage;

public class ClientObject : MonoBehaviour
{
    private static float speed = 5f;
    private static float deltaTime = 1f / LocalClient.Tick;
    private Quaternion desiredRotation;
    private Vector3 desiredPosition;

    private void Awake () {
        desiredRotation = transform.rotation;
        desiredPosition = transform.position;
    }
	
	private void Update () {
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, 0.5f);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, 0.5f);
    }

    public void SetDesiredValue(ExistingEntity existingEntity)
    {
        if (existingEntity != null)
        {
            desiredRotation = Optimazation.DecompressRot(existingEntity.rotation);
            desiredPosition = Optimazation.DecompressPos1(existingEntity.position);
        }
    }

    public void Predict(KeyCode code)
    {
        switch (code) {
            case KeyCode.W:
                transform.position += speed * deltaTime * Vector3.forward;
                desiredPosition = transform.position;
                break;
            case KeyCode.S:
                transform.position -= speed * deltaTime * Vector3.forward;
                desiredPosition = transform.position;
                break;
            case KeyCode.D:
                transform.position += speed * deltaTime * Vector3.right;
                desiredPosition = transform.position;
                break;
            case KeyCode.A:
                transform.position -= speed * deltaTime * Vector3.right;
                desiredPosition = transform.position;
                break;
            case KeyCode.Space:
                transform.Rotate(ServerObject.RotateSpeed * deltaTime);
                desiredRotation = transform.rotation;
                break;
        }
    }
}
