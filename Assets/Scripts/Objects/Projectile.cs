using UnityEngine;

public class Projectile : MonoBehaviour
{
    private const float Speed = 18f;
    public const int PrefabId = 2;
    public long id;
    public Vector3 direction;

    private MoveTrajectory MoveTrajectory = new MoveTrajectory();

    public void PrepareUpdate(Vector3 pos)
    {
        MoveTrajectory.Refresh(transform.position, pos, Speed);
    }

    public void GameUpdate(float deltaTime)
    {
        if (!MoveTrajectory.IsDone && !MoveTrajectory.CheckDone)
        {
            transform.position = MoveTrajectory.Update(deltaTime);
        }
    }
}
