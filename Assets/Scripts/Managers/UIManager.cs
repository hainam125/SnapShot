using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static UIManager Instance;

    public GameObject loginPanel;
    public GameObject roomPanel;

    private void Awake () {
        Instance = this;
	}
}
