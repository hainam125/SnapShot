using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkMessage;

public class GameManager : MonoBehaviour {
    public List<TestObject> syncObjects;
    
    public void UpdateState(SnapShot snapShot)
    {
        var entities = snapShot.existingEntities;
        for(int i = 0; i < syncObjects.Count; i++)
        {
            syncObjects[i].setDesiredValue(entities[i]);
        }
    }
}
