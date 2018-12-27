using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour {
    public bool usingUpdate;
    float speed = 4;
    bool allowInput;
    bool up;
    bool down;
    bool left;
    bool right;
    float deltaTime = 0.03f;
    // Use this for initialization
    private IEnumerator Start () {
        while (true)
        {
            allowInput = true;
            if (!usingUpdate)
            {
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
            }
            yield return deltaTime;
            allowInput = false;
        }
	}

    private void Update()
    {
        if (usingUpdate)
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += Vector3.forward * speed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                transform.position -= Vector3.forward * speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += Vector3.right * speed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                transform.position -= Vector3.right * speed * Time.deltaTime;
            }
        }
        if (allowInput && !usingUpdate)
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

    // Update is called once per frame
    void FixedUpdate1 () {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.forward * speed * Time.fixedDeltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position -= Vector3.forward * speed * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * speed * Time.fixedDeltaTime;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position -= Vector3.right * speed * Time.fixedDeltaTime;
        }
    }
}
