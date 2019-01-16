using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager Instance;

    public GameObject topPanel;
    public GameObject loginPanel;
    public GameObject roomPanel;

    private void Awake () {
        Instance = this;
	}

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }
}
