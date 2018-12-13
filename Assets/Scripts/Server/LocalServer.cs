using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkMessage;
using System;

public class LocalServer : MonoBehaviour
{
    public List<ServerObject> objects;
    public List<LocalClient> clients;
    private Dictionary<LocalClient, long> commandsSoFar = new Dictionary<LocalClient, long>();

    private IEnumerator Start()
    {
        foreach (var client in clients)
        {
            commandsSoFar[client] = 0;
        }

        var tick = 20;
        var time = 1.0f / tick;
        while (true)
        {
            var syncEntities = new List<ExistingEntity>();
            foreach (var gameObject in objects)
            {
                var entity = new ExistingEntity()
                {
                    position = Optimazation.CompressPos1(gameObject.transform.position),
                    rotation = Optimazation.CompressRot(gameObject.transform.rotation)
                };
                syncEntities.Add(entity);
            }
            var snapShot = new SnapShot() { existingEntities = syncEntities };
            foreach(var client in clients)
            {
                snapShot.commandId = commandsSoFar[client];
                StartCoroutine(client.ReceiveSnapShot(snapShot));
            }
            yield return new WaitForSeconds(time);
        }
    }

    public void ReceiveCommand(int objectIdx, Command command)
    {
        objects[objectIdx].ReceiveCommand(command);
        if (!commandsSoFar.ContainsKey(clients[objectIdx])) commandsSoFar[clients[objectIdx]] = 0;
        commandsSoFar[clients[objectIdx]] = command.id;
    }
}
