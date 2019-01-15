using System;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour {
    [SerializeField]
    private Button loginBtn;
    [SerializeField]
    private InputField nameField;
    [SerializeField]
    private User user;

    private void Start () {
        Action<Response> callback = (Response response) =>
        {
            var userData = JsonUtility.FromJson<UserData>(response.data);
            //Debug.Log("login successfully!: " + user.id);
            loginBtn.interactable = true;
            gameObject.SetActive(false);
            user.username = userData.username;
            UIManager.Instance.roomPanel.SetActive(true);
        };

        loginBtn.onClick.AddListener(() =>
        {
            string loginName = nameField.text;
            if (string.IsNullOrEmpty(loginName)) return;
            var user = new UserData(loginName);
            ConnectionManager.Send(new Request(JsonUtility.ToJson(user), typeof(UserData).ToString(), callback));
            nameField.text = string.Empty;
            loginBtn.interactable = false;
        });
	}
}
