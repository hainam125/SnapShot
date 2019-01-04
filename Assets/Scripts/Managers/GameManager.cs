using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkMessage;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string username;

    [SerializeField]
    private RemoteClient remoteClient;

    public List<Obstacle> obstacles = new List<Obstacle>();
    public List<PlayerObject> playerObjects = new List<PlayerObject>();

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;
        gameObject.SetActive(false);
        remoteClient.gameObject.SetActive(false);
    }

    public void Init(long objectId)
    {
        gameObject.SetActive(true);
        remoteClient.gameObject.SetActive(true);
        remoteClient.objectIndex = objectId;
        var playerObject = ObjectFactory.CreatePlayer1Object();
        playerObject.id = objectId;
        playerObject.SetName(username);

        remoteClient.AddObject(playerObject);
    }

    public void Init(CreateRoom createRoom)
    {
        var snapShot = JsonUtility.FromJson<SnapShot>(createRoom.snapShot);
        Init(createRoom.objectId);
        var newEntities = snapShot.newEntities;
        foreach (var e in newEntities)
        {
            if (e.prefabId == Obstacle.PrefabId)
            {
                obstacles.Add(ObjectFactory.CreateObstacle(e));
            }
        }
    }

    public void Init(EnterRoom roomData)
    {
        var snapShot = JsonUtility.FromJson<SnapShot>(roomData.snapShot);
        Init(roomData.objectId);
        var newEntities = snapShot.newEntities;
        var newObjects = new Dictionary<long, PlayerObject>();
        foreach (var e in newEntities)
        {
            if (e.prefabId == PlayerObject.PrefabId)
            {
                var playerObject = ObjectFactory.CreatePlayer2Object(e);

                remoteClient.AddObject(playerObject);
                newObjects.Add(playerObject.id, playerObject);
                playerObjects.Add(playerObject);
            }
            else if(e.prefabId == Obstacle.PrefabId)
            {
                obstacles.Add(ObjectFactory.CreateObstacle(e));
            }
            else if(e.prefabId == Projectile.PrefabId)
            {
                remoteClient.AddProjectile(ObjectFactory.CreateProjectile(e));
            }
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
        var playerObject = ObjectFactory.CreatePlayer2Object();
        playerObject.id = userJoined.objectId;
        playerObject.SetName(userJoined.username);
        remoteClient.AddObject(playerObject);
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
