using System;
using System.Collections;
using System.Collections.Generic;
using NetworkMessage;
using UnityEngine;

public class ServerObject : MonoBehaviour
{
    public long id;
    public static Vector3 RotateSpeed = new Vector3(0, 100f, 0);
    public const float Speed = 7f;
    public bool isDirty;
    private Queue<Command> commands = new Queue<Command>();

    public void ReceiveCommand(Command command)
    {
        commands.Enqueue(command);
    }

    public void UpdateGame()
    {
        while(commands.Count > 0) {
            HandleCommand(commands.Dequeue());
        }
    }

    private void HandleCommand(Command command)
    {
        isDirty = true;
        var deltaTime = BaseClient.DeltaTime;
        switch (command.keyCode)
        {
            case 0:
                transform.position += Speed * deltaTime * transform.forward;
                break;
            case 1:
                transform.position -= Speed * deltaTime * transform.forward;
                break;
            case 2:
                transform.Rotate(RotateSpeed * deltaTime);
                break;
            case 3:
                transform.Rotate(-RotateSpeed * deltaTime);
                break;
        }
    }
}
