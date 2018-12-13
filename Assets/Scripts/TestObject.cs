using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkMessage;

public class TestObject : MonoBehaviour {
    private Quaternion desiredRotation;
    private Vector3 desiredPosition;

    private void Awake () {
        desiredRotation = transform.rotation;
        desiredPosition = transform.position;
    }
	
	private void Update () {
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, 0.5f);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, 0.5f);
    }

    public void setDesiredValue(ExistingEntity existingEntity)
    {
        desiredRotation = Optimazation.DecompressRot(existingEntity.rotation);
        desiredPosition = Optimazation.DecompressPos1(existingEntity.position);// + new Vector3(2, 0, 0);
    }
}
