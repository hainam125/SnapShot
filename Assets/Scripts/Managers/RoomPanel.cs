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
    [SerializeField]
    private InputField playerNbField;
    
    private void Start()
    {
        SetDefaultValue();
        AddUIEvents();
    }

    private void RefreshRoom(List<string> rooms)
    {
        for (int i = 1; i < roomContainer.childCount; i++) Destroy(roomContainer.GetChild(i).gameObject);

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
            if (!string.IsNullOrEmpty(roomData.snapShot))
            {
                gameObject.SetActive(false);
                GameManager.Instance.Init(roomData);
                UIManager.Instance.Toggle(true);
            }
            else
            {
                Debug.Log("Cannot join room");
            }
        }));
    }

    private void AddUIEvents()
    {
        nameField.onEndEdit.AddListener((value) =>
        {
            createRoomBtn.interactable = IsValidRoomName() && IsValidPlayerNumber();
        });

        playerNbField.onEndEdit.AddListener((value) =>
        {
            createRoomBtn.interactable = IsValidRoomName() && IsValidPlayerNumber();
        });

        createRoomBtn.onClick.AddListener(() =>
        {
            string roomName = nameField.text;
            int nb = int.Parse(playerNbField.text);

            var room = new CreateRoom(roomName, nb);
            ConnectionManager.Send(new Request(JsonUtility.ToJson(room), typeof(CreateRoom).ToString(), (Response response) =>
            {
                var roomData = JsonUtility.FromJson<CreateRoom>(response.data);
                if (roomData.success)
                {
                    SetDefaultValue();
                    GameManager.Instance.Init(roomData);
                    UIManager.Instance.Toggle(true);
                }
                else
                {
                    Debug.Log("failed");
                }
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

    private void SetDefaultValue()
    {
        nameField.text = "MyRoom";
        playerNbField.text = "6";
        createRoomBtn.interactable = true;
    }

    private bool IsValidRoomName()
    {
        var value = nameField.text;
        if (string.IsNullOrEmpty(value)) return false;

        var regex = @"^[a-zA-Z0-9_]+$";
        var match = Regex.Match(value, regex, RegexOptions.IgnoreCase);
        return match.Success;
    }

    private bool IsValidPlayerNumber()
    {
        var value = playerNbField.text;
        if (string.IsNullOrEmpty(value)) return false;

        int nb = int.Parse(value);
        return nb > 0;
    }
}
