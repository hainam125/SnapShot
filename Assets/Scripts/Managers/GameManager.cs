using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetworkMessage;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField]
    private User user;
    [SerializeField]
    private RemoteClient remoteClient;
    [SerializeField]
    private Transform mCamera;

    private bool isCameraFollowing;
    public Transform CamTransform { get { return mCamera; } }

    public List<Obstacle> obstacles = new List<Obstacle>();
    public List<PlayerObject> playerObjects = new List<PlayerObject>();

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;

        SetActive(false);
    }

    private void Update()
    {
        if(isCameraFollowing)
        {
            var mainPlayer = remoteClient.GetMainPlayer().transform;
            mCamera.eulerAngles = new Vector3(30, mainPlayer.eulerAngles.y, 0f);
            mCamera.position = mainPlayer.position + new Vector3(0, 4.5f, 0) + mainPlayer.forward * -5.5f;
        }
    }

    private void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
        remoteClient.gameObject.SetActive(isActive);
    }

    private void Init(long objectId)
    {
        gameObject.SetActive(true);
        remoteClient.gameObject.SetActive(true);
        remoteClient.objectIndex = objectId;
        var playerObject = ObjectFactory.CreatePlayer1Object();
        playerObject.SetId(objectId);
        playerObject.view.SetName(user.username);
        playerObject.SetHp();

        remoteClient.AddObject(playerObject);
        ToggleCamera(false);
    }

    public void Init(CreateRoom createRoom)
    {
        var snapShot = JsonUtility.FromJson<SnapShot>(createRoom.snapShot);
        Init(createRoom.objectId);
        var newEntities = snapShot.newEntities;
        foreach (var e in newEntities)
        {
            if (e.prefabId == Config.ObstaclePrefabId)
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
        var newPlayers = snapShot.newPlayers;
        var newObjects = new Dictionary<long, PlayerObject>();
        foreach (var e in newEntities)
        {
            if (e.prefabId == Config.ObstaclePrefabId)
            {
                obstacles.Add(ObjectFactory.CreateObstacle(e));
            }
            else if(e.prefabId == Config.ProjectilePrefabId)
            {
                remoteClient.AddProjectile(ObjectFactory.CreateProjectile(e));
            }
        }
        foreach (var e in newPlayers)
        {
            if (e.prefabId == Config.PlayerPrefabId)
            {
                var playerObject = ObjectFactory.CreatePlayer2Object(e);
                playerObject.SetHp(e.hp);
                remoteClient.AddObject(playerObject);
                newObjects.Add(playerObject.Id, playerObject);
                playerObjects.Add(playerObject);
            }
        }
        for (int i = 0; i < roomData.ids.Count; i++)
        {
            newObjects[roomData.ids[i]].view.SetName(roomData.usernames[i]);
        }
    }

    public void Clear()
    {
        obstacles.Clear();
        playerObjects.Clear();
        remoteClient.Clear();
        ObjectFactory.Clear();
        SetActive(false);
    }

    public void Join(Response response)
    {
        var userJoined = JsonUtility.FromJson<UserJoined>(response.data);
        Debug.Log("User joined: " + userJoined.username);
        var playerObject = ObjectFactory.CreatePlayer2Object();
        playerObject.SetId(userJoined.objectId);
        playerObject.view.SetName(userJoined.username);
        playerObject.SetHp();
        remoteClient.AddObject(playerObject);
        playerObjects.Add(playerObject);
    }

    public void Exit(Response response)
    {
        var userExited = JsonUtility.FromJson<UserExited>(response.data);
        var player = remoteClient.RemoveObject(userExited.objectId);
        playerObjects.Remove(player);
        Debug.Log("User exit: " + userExited.username);
    }

    public void ReceiveSnapShot(Response response)
    {
        var snapShot = JsonUtility.FromJson<SnapShot>(response.data);
        remoteClient.ReceiveSnapShot(snapShot);
    }

    public void ToggleCamera(bool isFixed)
    {
        if (remoteClient.objectIndex < 0) return;
        isCameraFollowing = !isFixed;
        if (isFixed)
        {
            mCamera.position = new Vector3(0, 20, -20);
            mCamera.eulerAngles = new Vector3(50, 0, 0);
        }
    }
}
