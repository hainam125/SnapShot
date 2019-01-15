using NetworkMessage;
using UnityEngine;

public class ObjectFactory : MonoBehaviour {
    private static ObjectFactory instance;

    [SerializeField]
    private Transform projectiles;
    [SerializeField]
    private Transform players;
    [SerializeField]
    private Transform obstacles;
    [SerializeField]
    private GameObject objectPrefab;
    [SerializeField]
    private GameObject obstaclePrefab;
    [SerializeField]
    private GameObject projectilePrefab;
   
    private void Awake () {
        instance = this;
    }

    public static PlayerObject CreatePlayer1Object()
    {
        var player = Instantiate(instance.objectPrefab, instance.players).GetComponent<PlayerObject>();
        player.Init(Config.PlayerRotateSpeed, Config.PlayerMoveSpeed, Config.PlayerMaxHP, Config.fireRate);
        return player;
    }

    public static PlayerObject CreatePlayer2Object()
    {
        var player = Instantiate(instance.objectPrefab, instance.players).GetComponent<PlayerObject>();
        player.Init(Config.PlayerRotateSpeed, Config.PlayerMoveSpeed, Config.PlayerMaxHP, Config.fireRate);
        return player;
    }

    public static PlayerObject CreatePlayer2Object(NewEntity e)
    {
        var playerObject = CreatePlayer2Object();
        playerObject.SetId(e.id);
        var rot = Optimazation.DecompressRot(e.rotation);
        var pos = Optimazation.DecompressPos2(e.position);
        playerObject.transform.position = pos;
        playerObject.transform.rotation = rot;
        return playerObject;
    }

    public static Obstacle CreateObstacle(NewEntity e)
    {
        var obstacle = Instantiate(instance.obstaclePrefab, instance.obstacles).GetComponent<Obstacle>();
        var rot = Optimazation.DecompressRot(e.rotation);
        var pos = Optimazation.DecompressPos2(e.position);
        var bound = Optimazation.DecompressPos2(e.bound);
        obstacle.transform.position = pos;
        obstacle.transform.rotation = rot;
        obstacle.transform.localScale = bound;
        return obstacle;
    }

    public static Projectile CreateProjectile(NewEntity e)
    {
        var projectile = Instantiate(instance.projectilePrefab, instance.projectiles).GetComponent<Projectile>();
        var rot = Optimazation.DecompressRot(e.rotation);
        var pos = Optimazation.DecompressPos2(e.position);
        var bound = Optimazation.DecompressPos2(e.bound);
        projectile.SetId(e.id);
        projectile.transform.position = pos;
        projectile.transform.rotation = rot;
        projectile.transform.localScale = bound;
        return projectile;
    }
}
