using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginObject : MonoBehaviour {
    private static float speed = 5f;

	private void Update () {
        if (Input.GetKey(KeyCode.W)) transform.position += speed * Time.deltaTime * Vector3.forward;
        else if (Input.GetKey(KeyCode.S)) transform.position -= speed * Time.deltaTime * Vector3.forward;
        if (Input.GetKey(KeyCode.D)) transform.position += speed * Time.deltaTime * Vector3.right;
        else if (Input.GetKey(KeyCode.A)) transform.position -= speed * Time.deltaTime * Vector3.right;
        if (Input.GetKey(KeyCode.Space)) transform.Rotate(new Vector3(0, 90f, 0) * Time.deltaTime);
    }
}
