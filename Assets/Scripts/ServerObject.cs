using System;
using System.Collections;
using System.Collections.Generic;
using NetworkMessage;
using UnityEngine;

public class ServerObject : MonoBehaviour {
    private static float speed = 5f;
    
    public void ReceiveCommand(Command command)
    {
        switch(command.keyCode)
        {
            case 0:
                transform.position += speed * Time.deltaTime * Vector3.forward;
                break;
            case 1:
                transform.position -= speed * Time.deltaTime * Vector3.forward;
                break;
            case 2:
                transform.position += speed * Time.deltaTime * Vector3.right;
                break;
            case 3:
                transform.position -= speed * Time.deltaTime * Vector3.right;
                break;
            case 4:
                transform.Rotate(new Vector3(0, 100f, 0) * Time.deltaTime);
                break;
        }
    }
}
