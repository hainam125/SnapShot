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
    private GameObject obstaclePrefab;
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private RemoteClient remoteClient;

    public List<Obstacle> obstacles = new List<Obstacle>();
    public List<ClientObject> objects = new List<ClientObject>();

    private void Awake()
    {
        Application.targetFrameRate = 60;
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

    public void Init(CreateRoom createRoom)
    {
        var snapShot = JsonUtility.FromJson<SnapShot>(createRoom.snapShot);
        Init(createRoom.objectId);
        var newEntities = snapShot.newEntities;
        var newObjects = new Dictionary<long, ClientObject>();
        foreach (var e in newEntities)
        {
            if (e.prefabId == Obstacle.PrefabId)
            {
                CreateObstacle(e);
            }
        }
    }

    public void Init(EnterRoom roomData)
    {
        var snapShot = JsonUtility.FromJson<SnapShot>(roomData.snapShot);
        Init(roomData.objectId);
        var newEntities = snapShot.newEntities;
        var newObjects = new Dictionary<long, ClientObject>();
        foreach (var e in newEntities)
        {
            if (e.prefabId == ClientObject.PrefabId)
            {
                var clientObj = Instantiate(object2Prefab, environment).GetComponent<ClientObject>();
                clientObj.id = e.id;
                var rot = Optimazation.DecompressRot(e.rotation);
                var pos = Optimazation.DecompressPos2(e.position);
                clientObj.transform.position = clientObj.desiredPosition = pos;
                clientObj.transform.rotation = clientObj.desiredRotation = rot;

                remoteClient.AddObject(clientObj);
                newObjects.Add(clientObj.id, clientObj);
                objects.Add(clientObj);
            }
            else if(e.prefabId == Obstacle.PrefabId)
            {
                CreateObstacle(e);
            }
            else if(e.prefabId == 2)
            {
                Debug.Log("Projectile!!");
            }
        }
        for(int i = 0; i < roomData.ids.Count; i++)
        {
            newObjects[roomData.ids[i]].SetName(roomData.usernames[i]);
        }
    }

    private void CreateObstacle(NewEntity e)
    {
        var obsObj = Instantiate(obstaclePrefab, environment).GetComponent<Obstacle>();
        var rot = Optimazation.DecompressRot(e.rotation);
        var pos = Optimazation.DecompressPos2(e.position);
        var bound = Optimazation.DecompressPos2(e.bound);
        obsObj.transform.position = pos;
        obsObj.transform.rotation = rot;
        obsObj.transform.localScale = bound;
        obstacles.Add(obsObj);
    }

    public Projectile CreateProjectile(NewEntity e)
    {
        var obsObj = Instantiate(projectilePrefab, environment).GetComponent<Projectile>();
        var rot = Optimazation.DecompressRot(e.rotation);
        var pos = Optimazation.DecompressPos2(e.position);
        var bound = Optimazation.DecompressPos2(e.bound);
        obsObj.id = e.id;
        obsObj.transform.position = pos;
        obsObj.transform.rotation = rot;
        obsObj.transform.localScale = bound;
        return obsObj;
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
