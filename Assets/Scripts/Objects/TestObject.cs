using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObject : MonoBehaviour {

	private IEnumerator Start () {
        float direction = 1;
		while(true)
        {
            float speed = 2f;
            transform.position += Vector3.up * Time.deltaTime * speed * direction;
            if (transform.position.y > 2.5f) direction = -1;
            else if (transform.position.y < 0.5f) direction = 1;
            yield return null;
        }
	}
}
