using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IPPanel : MonoBehaviour
{
    [SerializeField]
    private InputField urlField;
    [SerializeField]
    private Button connectBtn;

    private void Start()
    {
        Action success = () =>
        {
            Debug.Log("Connected...!");
            UniRx.MainThreadDispatcher.Send((object o) =>
            {
                gameObject.SetActive(false);
                UIManager.Instance.loginPanel.SetActive(true);
            }, null);
        };

        Action fail = () =>
        {
            Debug.Log("Disconnected!");
            UniRx.MainThreadDispatcher.Send((object o) =>
            {
                connectBtn.interactable = true;
            }, null);
        };

        connectBtn.onClick.AddListener(() =>
        {
            string url = urlField.text;
            if (string.IsNullOrEmpty(url)) return;
            ConnectionManager.Connect(url, success, fail);
            connectBtn.interactable = false;
        });
    }
}
