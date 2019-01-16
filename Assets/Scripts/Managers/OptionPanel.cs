using UnityEngine;
using UnityEngine.UI;

public class OptionPanel : MonoBehaviour {
    [SerializeField]
    private Button leaveRoomBtn;

    private void Start()
    {
        leaveRoomBtn.onClick.AddListener(() =>
        {
            ConnectionManager.Send(new Request(string.Empty, typeof(LeaveRoom).ToString(), (Response response) =>
            {
                UIManager.Instance.Toggle(false);
                GameManager.Instance.Clear();
            }));
        });
    }
}
