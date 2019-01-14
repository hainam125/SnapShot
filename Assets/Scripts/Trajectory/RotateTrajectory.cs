using UnityEngine;

public class RotateTrajectory {

    #region ===== Fields =====
    private const float totalTime = Constant.ServerDeltaTime;
    private Quaternion rotation;
    private Quaternion start;
    private Quaternion end;
    private bool isDone = true;
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
