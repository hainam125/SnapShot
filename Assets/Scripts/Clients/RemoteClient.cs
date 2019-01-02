using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetworkMessage;

public class RemoteClient : BaseClient
{
	[SerializeField]
    private Toggle predictionToggle;
	[SerializeField]
    private Toggle reconcilationToggle;
	[SerializeField]
	private Toggle interpolationToggle;

    private void Awake()
    {
        ServerDeltaTime = 1f / 15f;
		predictionToggle.onValueChanged.AddListener(value => prediction = value);
		reconcilationToggle.onValueChanged.AddListener(value => reconcilation = value);
		interpolationToggle.onValueChanged.AddListener(value => entityInterpolation = value);
    }

    public void AddObject(ClientObject clientObject)
    {
        objectDict.Add(clientObject.id, clientObject);
    }

    public void RemoveObject(long objectId)
    {
        Destroy(objectDict[objectId].gameObject);
        objectDict.Remove(objectId);
    }

    public void ReceiveSnapShot(SnapShot snapShot)
    {
        ProcessSnapShot(snapShot);
    }

    protected override void InputUpdate()
    {
        ProcessInput();
    }

    public override IEnumerator SendCommand(Command command)
    {
        ConnectionManager.Send(new Request(JsonUtility.ToJson(command), typeof(Command).Name.ToString()));
        yield return null;
    }
}
