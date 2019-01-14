using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTrajectory {

    #region ===== Fields =====
    protected Quaternion rotation;
    protected Quaternion start;
    protected Quaternion end;
    private bool isDone = true;
    private float totalTime = BaseClient.ServerDeltaTime;
    private float currentTime;
    #endregion

    #region ===== Properties =====
    public bool IsDone { get { return isDone; } }

    #endregion

    #region ===== Methods =====

    public void Refresh(Quaternion begin, Quaternion finish)
    {
        isDone = false;
        start = begin;
        end = finish;
        currentTime = 0f;
    }

    public Quaternion Update(float dt)
    {
        currentTime += dt;
        if (currentTime < totalTime)
        {
            rotation = Quaternion.Slerp(start, end, currentTime / totalTime);
        }
        else
        {
            isDone = true;
            rotation = end;
        }
        return rotation;
    }
    #endregion
}
