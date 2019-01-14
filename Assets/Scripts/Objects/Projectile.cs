using UnityEngine;

public class Projectile : MonoBehaviour
{
    public long Id { get { return id; } }

    private long id;
    private MoveTrajectory MoveTrajectory = new MoveTrajectory();

    public void SetId(long newId)
    {
        id = newId;
    }

    public void PrepareUpdate(Vector3 pos)
    {
        MoveTrajectory.Refresh(transform.position, pos, Config.ProjectileSpeed);
    }

    public void GameUpdate(float deltaTime)
    {
        if (!MoveTrajectory.IsDone && !MoveTrajectory.CheckDone)
        {
            transform.position = MoveTrajectory.Update(deltaTime);
        }
    }
}
