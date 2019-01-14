using UnityEngine;

public class Projectile : MonoBehaviour
{
    public long Id { get { return id; } }

    private long id;
    private MoveTrajectory moveTrajectory = new MoveTrajectory();

    public void SetId(long newId)
    {
        id = newId;
    }

    public void UpdateState(Vector3 pos)
    {
        transform.position = pos;
    }

    public void PrepareUpdate(Vector3 pos)
    {
        moveTrajectory.Refresh(transform.position, pos, Config.ProjectileSpeed);
    }

    public void GameUpdate(float deltaTime)
    {
        if (!moveTrajectory.IsDone && !moveTrajectory.CheckDone)
        {
            transform.position = moveTrajectory.Update(deltaTime);
        }
    }
}
