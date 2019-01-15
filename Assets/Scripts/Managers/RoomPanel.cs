using NetworkMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : MonoBehaviour {
    [SerializeField]
    private GameObject buttonPrefab;

    [SerializeField]
    private Transform roomContainer;
    [SerializeField]
    private Button getRoomBtn;
    [SerializeField]
    private Button createRoomBtn;
    [SerializeField]
    private InputField nameField;

    private void Start()
    {
        nameField.onEndEdit.AddListener((value) =>
        {
            if (string.IsNullOrEmpty(value)) createRoomBtn.interactable = false;

            var regex = @"^[a-zA-Z0-9_]+$";
            var match = Regex.Match(value, regex, RegexOptions.IgnoreCase);
            createRoomBtn.interactable = match.Success;
        });
        createRoomBtn.onClick.AddListener(() =>
        {
            string roomName = nameField.text;

            var room = new CreateRoom(roomName);
            ConnectionManager.Send(new Request(JsonUtility.ToJson(room), typeof(CreateRoom).ToString(), (Response response) =>
            {
                var roomData = JsonUtility.FromJson<CreateRoom>(response.data);
                if (roomData.success)
                {
                    //Debug.Log("create room successfully!: " + roomData.name);
                    gameObject.SetActive(false);
                    GameManager.Instance.Init(roomData);
                    UIManager.Instance.TurnOff();
                }
                else
                {
                    Debug.Log("failed");
                }
                createRoomBtn.interactable = true;
            }));
            nameField.text = string.Empty;
            createRoomBtn.interactable = false;
        });


        getRoomBtn.onClick.AddListener(() =>
        {
            ConnectionManager.Send(new Request(string.Empty, typeof(RoomList).ToString(), (Response response) =>
            {
                var data = JsonUtility.FromJson<RoomList>(response.data);
                //Debug.Log("create room successfully!: " + data.rooms.Count);
                RefreshRoom(data.rooms);
                getRoomBtn.interactable = true;
            }));
            getRoomBtn.interactable = false;
        });
        getRoomBtn.onClick.Invoke();
    }

    private void RefreshRoom(List<string> rooms)
    {
        foreach (var roomName in rooms)
        {
            Button btn = Instantiate(buttonPrefab, roomContainer).GetComponent<Button>();
            btn.gameObject.SetActive(true);
            btn.transform.GetChild(0).GetComponent<Text>().text = roomName;
            btn.onClick.AddListener(() =>
            {
                JoinRoom(roomName);
            });
        }
    }

    private void JoinRoom(string roomName)
    {
        ConnectionManager.Send(new Request(JsonUtility.ToJson(new EnterRoom(roomName)), typeof(EnterRoom).ToString(), (Response response) =>
        {
            var roomData = JsonUtility.FromJson<EnterRoom>(response.data);
            gameObject.SetActive(false);
            GameManager.Instance.Init(roomData);
            UIManager.Instance.TurnOff();
        }));
    }
}
