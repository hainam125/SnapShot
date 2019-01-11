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
    #endregion

    #region ===== Properties =====

    public bool IsDone
    {
        get { return spentTime >= duration; }
    }

    public float Percent
    {
        get { return Mathf.Clamp01(spentTime / duration); }
    }

    #endregion

    #region ===== Methods =====

    public MoveTrajectory(Vector3 start, Vector3 end, float speed)
    {
        this.start = start;
        this.end = end;
        this.speed = speed;
        this.position = start;
        this.duration = Vector3.Distance(end,start)/speed;
        this.spentTime = 0f;
        this.direction = (end - start).normalized;
    }

    public Vector3 Update(float dt)
    {
        if (duration == 0)
            return end;

        spentTime = Mathf.Min(spentTime + dt, duration);

        position += direction * dt * speed;
        
        return IsDone ? end : position;
    }

    #endregion
}
