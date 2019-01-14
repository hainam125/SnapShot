using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTrajectory
{
    #region ===== Fields =====
    protected Vector3 position = Vector3.zero;
    protected Vector3 start = Vector3.zero;
    protected Vector3 end = Vector3.zero;
    protected float duration = 0f;
    protected float speed = 0f;
    protected float spentTime = 0f;
    protected Vector3 direction = Vector3.zero;
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
