using UnityEngine;

public class MoveTrajectory
{
    #region ===== Fields =====
    private Vector3 position = Vector3.zero;
    private Vector3 start = Vector3.zero;
    private Vector3 end = Vector3.zero;
    private float duration = 0f;
    private float speed = 0f;
    private float spentTime = 0f;
    private Vector3 direction = Vector3.zero;
    private bool isDone = true;
    #endregion

    #region ===== Properties =====
    public bool IsDone { get { return isDone; } }

    public bool CheckDone
    {
        get {
            if (spentTime >= duration)
            {
                isDone = true;
                return true;
            }
            return false;
        }
    }

    #endregion

    #region ===== Methods =====

    public void Refresh(Vector3 begin, Vector3 finish, float spd)
    {
        isDone = false;
        start = begin;
        end = finish;
        position = start;
        speed = spd;
        duration = Vector3.Distance(end, start) / speed;
        spentTime = 0f;
        direction = (end - start).normalized;
    }

    public Vector3 Update(float dt)
    {
        if (duration == 0) return end;

        spentTime = Mathf.Min(spentTime + dt, duration);

        position += direction * dt * speed;
        
        return CheckDone ? end : position;
    }

    #endregion
}
