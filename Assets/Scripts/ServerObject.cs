using System;
using System.Collections;
using System.Collections.Generic;
using NetworkMessage;
using UnityEngine;

public class ServerObject : MonoBehaviour
{
    public static Vector3 RotateSpeed = new Vector3(0, 100f, 0);
    private static float speed = 5f;
    private static float deltaTime = 1f / LocalClient.Tick;
    public bool isDirty;

    public void ReceiveCommand(Command command)
    {
        isDirty = true;
        switch (command.keyCode)
        {
            case 0:
                transform.position += speed * deltaTime * Vector3.forward;
                break;
            case 1:
                transform.position -= speed * deltaTime * Vector3.forward;
                break;
            case 2:
                transform.position += speed * deltaTime * Vector3.right;
                break;
            case 3:
                transform.position -= speed * deltaTime * Vector3.right;
                break;
            case 4:
                transform.Rotate(RotateSpeed * deltaTime);
                break;
        }
    }
}
