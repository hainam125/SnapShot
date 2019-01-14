using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour {
    [SerializeField]
    private Text nameTxt;
    [SerializeField]
    private RectTransform hpRect;
    [SerializeField]
    private GameObject mCamera;

    public void SetName(string name)
    {
        nameTxt.transform.parent.gameObject.SetActive(true);
        nameTxt.text = name;
    }

    public void UpdateHp(float hpPercentage)
    {
        hpRect.localScale = new Vector3(hpPercentage, 1f, 1f);
    }

    public void ToggleCamera(bool active)
    {
        mCamera.SetActive(active);
    }
}
