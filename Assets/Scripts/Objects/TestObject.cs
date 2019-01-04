using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObject : MonoBehaviour {

	private IEnumerator Start () {
        float direction = 1f;
		while(true)
        {
            float speed = 1.5f;
            float deltaTime = 0.05f;
            transform.position += Vector3.up * deltaTime * speed * direction;
            if (transform.position.y > 3.5f) direction = -1f;
            else if (transform.position.y < 1.5f) direction = 1f;
            yield return deltaTime;
        }
	}
}
