using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove1 : MonoBehaviour
{
    float speed = 4;
    bool up;
    bool down;
    bool left;
    bool right;

    void Update()
    {
        if (Input.GetKey(KeyCode.W)) {
            up = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            down = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            right = true;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            left = true;
        }
    }

    void FixedUpdate()
    {
        if (up)
        {
            up = false;
            transform.position += Vector3.forward * speed * Time.fixedDeltaTime;
        }
        else if (down)
        {
            down = false;
            transform.position -= Vector3.forward * speed * Time.fixedDeltaTime;
        }
        if (right)
        {
            right = false;
            transform.position += Vector3.right * speed * Time.fixedDeltaTime;
        }
        else if (left)
        {
            left = false;
            transform.position -= Vector3.right * speed * Time.fixedDeltaTime;
        }
    }
}
