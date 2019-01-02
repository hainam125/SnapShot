using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public const int PrefabId = 2;
    public long id;
    public Vector3 direction;

    private bool needUpdate;
    private float currentUpTime;
    private Vector3 startPos;
    private Vector3 targetPos;

    public void PrepareUpdate(Vector3 pos)
    {
        needUpdate = true;
        currentUpTime = 0f;
        targetPos = pos;
        startPos = transform.position;
    }

    public void GameUpdate(float deltaTime)
    {
        if (!needUpdate) return;
        var totalTime = BaseClient.ServerDeltaTime;
        if (currentUpTime < totalTime)
        {
            currentUpTime += deltaTime;
            var nextTime = currentUpTime + deltaTime;
            if (nextTime < totalTime)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, currentUpTime / totalTime);
            }
            else
            {
                transform.position = targetPos;
                needUpdate = false;
            }
        }
    }
}
