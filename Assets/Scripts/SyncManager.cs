using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkMessage;

public class SyncManager : MonoBehaviour {
    public List<GameObject> syncObjects;
    public GameManager manager;

    private IEnumerator Start()
    {
        var tick = 20;
        var time = 1.0f / tick;
        while (true)
        {
            var syncEntities = new List<ExistingEntity>();
            foreach (var gameObject in syncObjects)
            {
                var entity = new ExistingEntity()
                {
                    position = Optimazation.CompressPos1(gameObject.transform.position),
                    rotation = Optimazation.CompressRot(gameObject.transform.rotation)
                };
                syncEntities.Add(entity);
            }
            var snapShot = new SnapShot() { existingEntities = syncEntities };
            manager.UpdateState(snapShot);
            yield return new WaitForSeconds(time);
        }
    }

    private void Update () {
		
	}
}
