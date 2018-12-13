using System;
using System.Collections;
using System.Collections.Generic;
using NetworkMessage;
using UnityEngine;

public class ServerObject : MonoBehaviour {
    private static float speed = 5f;

	/*private void Update () {
        if (Input.GetKey(KeyCode.W)) transform.position += speed * Time.deltaTime * Vector3.forward;
        else if (Input.GetKey(KeyCode.S)) transform.position -= speed * Time.deltaTime * Vector3.forward;
        if (Input.GetKey(KeyCode.D)) transform.position += speed * Time.deltaTime * Vector3.right;
        else if (Input.GetKey(KeyCode.A)) transform.position -= speed * Time.deltaTime * Vector3.right;
        if (Input.GetKey(KeyCode.Space)) transform.Rotate(new Vector3(0, 90f, 0) * Time.deltaTime);
    }*/

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
