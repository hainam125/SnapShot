using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager Instance;

    public GameObject outGame;
    public GameObject topPanel;
    public GameObject loginPanel;
    public GameObject roomPanel;

    public GameObject inGame;

    private void Awake () {
        Instance = this;
        Toggle(false);
	}

    public void Toggle(bool isInGame)
    {
        outGame.SetActive(!isInGame);
        inGame.SetActive(isInGame);
    }
}
