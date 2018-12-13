using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkMessage;
using System;

public class LocalServer : MonoBehaviour
{
    public List<OriginObject> objects;
    public List<LocalClient> clients;

    private IEnumerator Start()
    {
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
                StartCoroutine(client.ReceiveSnapShot(snapShot));
            }
            yield return new WaitForSeconds(time);
        }
    }

    public void ReceiveCommand(int objectIdx, Command command)
    {
        throw new NotImplementedException();
    }
}
