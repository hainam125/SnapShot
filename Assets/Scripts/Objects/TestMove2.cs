using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove2 : MonoBehaviour {
    float speed = 4;
    bool allowInput;
    bool up;
    bool down;
    bool left;
    bool right;
    float deltaTime = 0.03f;

    private IEnumerator Start()
    {
        while (true)
        {
            allowInput = true;
            if (up)
            {
                transform.position += Vector3.forward * speed * deltaTime;
                up = false;
            }
            if (down)
            {
                transform.position -= Vector3.forward * speed * deltaTime;
                down = false;
            }
            if (right)
            {
                transform.position += Vector3.right * speed * deltaTime;
                right = false;
            }
            if (left)
            {
                transform.position -= Vector3.right * speed * deltaTime;
                left = false;
            }
            yield return deltaTime;
            allowInput = false;
        }
    }

    private void Update()
    {
        if (allowInput)
        {
            if (Input.GetKey(KeyCode.W))
            {
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
    }
}
