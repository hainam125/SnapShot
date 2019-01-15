using UnityEngine;

public static class Config {
    public const float ProjectileSpeed = 18f;
    public const int PlayerMaxHP = 5;
    public static Vector3 PlayerRotateSpeed = new Vector3(0, 50f, 0);
    public const float PlayerMoveSpeed = 7f;
    public const float fireRate = 2.0f;

    public const int PlayerPrefabId = 0;
    public const int ObstaclePrefabId = 1;
    public const int ProjectilePrefabId = 2;
}

public static class Constant
{
    public const float ServerDeltaTime = 1f / 15f;
}
