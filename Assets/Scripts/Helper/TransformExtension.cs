using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension
{
    public static List<Vector3> GetPointsInWorldCord(this Transform transform, Vector3? position=null)
    {
        var bound = transform.localScale;
        Vector3 pos = (position != null) ?  (Vector3)position : transform.position;
        List<Vector3> points = new List<Vector3>(){
            new Vector3(-bound.x * 0.5f, 0f, bound.z * 0.5f),
            new Vector3(bound.x * 0.5f, 0f, bound.z * 0.5f),
            new Vector3(bound.x * 0.5f, 0f, -bound.z * 0.5f),
            new Vector3(-bound.x * 0.5f, 0f, -bound.z * 0.5f)
        };

        Matrix3x3 matrix = Matrix3x3.One;
        matrix.RotateY(transform.eulerAngles.y / 180f * Mathf.PI);
        matrix.TranslateY(pos.x, pos.z);
        points = matrix.TransformY(points);

        return points;
    }

    public static List<Vector3> GetLocalPoint(this Transform transform, List<Vector3> points, Vector3? position = null)
    {
        Matrix3x3 matrix = Matrix3x3.One;
        if (position == null) position = transform.position;
        for (int i = 0; i < points.Count; i++) points[i] -= (Vector3)position;

        matrix.RotateY(-transform.eulerAngles.y / 180f * Mathf.PI);
        points = matrix.TransformY(points);
        return points;
    }

    public static bool CheckCollision(this Transform transform, Transform other, Vector3? position = null, Vector3? otherPosition = null)
    {
        List<Vector3> points = transform.GetLocalPoint(other.GetPointsInWorldCord(otherPosition), position);
        var bound = transform.localScale;
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 point = points[i];
            if (point.x <= bound.x * 0.5f && point.x >= -bound.x * 0.5f && point.z <= bound.z * 0.5f && point.z >= -bound.z * 0.5f) return true;
        }
        points = other.GetLocalPoint(transform.GetPointsInWorldCord(position), otherPosition);
        bound = other.localScale;
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 point = points[i];
            if (point.x <= bound.x * 0.5f && point.x >= -bound.x * 0.5f && point.z <= bound.z * 0.5f && point.z >= -bound.z * 0.5f) return true;
        }
        return false;
    }
}
