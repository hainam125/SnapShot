using NetworkMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string username;

[	SerializeField]
	private Transform environment;
    [SerializeField]
    private GameObject object1Prefab;
    [SerializeField]
    private GameObject object2Prefab;
    [SerializeField]
    private RemoteClient remoteClient;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
        remoteClient.gameObject.SetActive(false);
    }

    private void Update()
    {

    }

    public void Init(long objectId)
    {
        gameObject.SetActive(true);
        remoteClient.gameObject.SetActive(true);
        remoteClient.objectIndex = objectId;
        var clientObject = Instantiate(object1Prefab, environment).GetComponent<ClientObject>();
        clientObject.id = objectId;
        clientObject.SetName(username);

        remoteClient.AddObject(clientObject);
    }

    public void Init(EnterRoom roomData)
    {
        var snapShot = JsonUtility.FromJson<SnapShot>(roomData.snapShot);
        Init(roomData.objectId);
        var newEntities = snapShot.newEntities;
        var newObjects = new Dictionary<long, ClientObject>();
        foreach (var e in newEntities)
        {
            var clientObj = Instantiate(object2Prefab, environment).GetComponent<ClientObject>();
            clientObj.id = e.id;
            var rot = Optimazation.DecompressRot(e.rotation);
            var pos = Optimazation.DecompressPos2(e.position);
            clientObj.transform.position = clientObj.desiredPosition = pos;
            clientObj.transform.rotation = clientObj.desiredRotation = rot;

            remoteClient.AddObject(clientObj);
            newObjects.Add(clientObj.id, clientObj);
        }
        for(int i = 0; i < roomData.ids.Count; i++)
        {
            newObjects[roomData.ids[i]].SetName(roomData.usernames[i]);
        }
    }

    public void Join(Response response)
    {
        var userJoined = JsonUtility.FromJson<UserJoined>(response.data);
        Debug.Log("User joined: " + userJoined.username);
        var clientObject = Instantiate(object2Prefab, environment).GetComponent<ClientObject>();
        clientObject.id = userJoined.objectId;
        clientObject.SetName(userJoined.username);
        remoteClient.AddObject(clientObject);
    }

    public void Exit(Response response)
    {
        var userJoined = JsonUtility.FromJson<UserExited>(response.data);
        remoteClient.RemoveObject(userJoined.objectId);
        Debug.Log("User exit: " + userJoined.username);
    }

    public void ReceiveSnapShot(Response response)
    {
        var snapShot = JsonUtility.FromJson<SnapShot>(response.data);
        remoteClient.ReceiveSnapShot(snapShot);
    }
}
