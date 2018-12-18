using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkMessage;
using UnityEngine.UI;

public class LocalServer : MonoBehaviour
{
    public int tick = 20;
    public List<ServerObject> objects;
    public List<LocalClient> clients;
    private Dictionary<LocalClient, long> commandsSoFar = new Dictionary<LocalClient, long>();
    private List<SnapShot> snapShots = new List<SnapShot>();
    private bool recording;
    public Slider slider;
    public Button button;

    private IEnumerator Start()
    {
        recording = true;
        button.onClick.AddListener(() => { recording = false; });
        slider.onValueChanged.AddListener((value) =>
        {
            int idx = (int)(value * snapShots.Count);
            var entities = snapShots[idx].existingEntities;
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].transform.rotation = Optimazation.DecompressRot(entities[i].rotation);
                objects[i].transform.position = Optimazation.DecompressPos1(entities[i].position);
            }
        });

        foreach (var client in clients)
        {
            commandsSoFar[client] = 0;
        }
        
        var time = 1.0f / tick;
        var waitTime = new WaitForSeconds(time);
        while (true)
        {
            foreach (var gameObject in objects)
            {
                gameObject.UpdateGame();
            }

            var syncEntities = new List<ExistingEntity>();
            foreach (var gameObject in objects)
            {
                if (!gameObject.isDirty)
                {
                    syncEntities.Add(null);
                }
                else
                {
                    var entity = new ExistingEntity()
                    {
                        position = Optimazation.CompressPos1(gameObject.transform.position),
                        rotation = Optimazation.CompressRot(gameObject.transform.rotation)
                    };
                    syncEntities.Add(entity);
                    gameObject.isDirty = false;
                }
            }
            var snapShot = new SnapShot() { existingEntities = syncEntities };
            foreach(var client in clients)
            {
                var clone = snapShot.Clone();
                clone.commandId = commandsSoFar[client];
                StartCoroutine(client.ReceiveSnapShot(clone));
            }
            if(recording) snapShots.Add(snapShot);
            yield return waitTime;
        }
    }

    public void ReceiveCommand(int objectIdx, Command command)
    {
        objects[objectIdx].ReceiveCommand(command);
        if (!commandsSoFar.ContainsKey(clients[objectIdx])) commandsSoFar[clients[objectIdx]] = 0;
        commandsSoFar[clients[objectIdx]] = command.id;
    }
}
