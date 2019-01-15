using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour {
    [SerializeField]
    private Text nameTxt;
    [SerializeField]
    private Slider hpRect;
    [SerializeField]
    private GameObject tank;
    [SerializeField]
    private GameObject explosion;
    [SerializeField]
    private GameObject mCamera;

    public void SetName(string name)
    {
        nameTxt.transform.parent.gameObject.SetActive(true);
        nameTxt.text = name;
    }

    public void UpdateHp(float hpPercentage)
    {
        hpRect.value = hpPercentage * 100f;
    }

    public void ToggleCamera(bool active)
    {
        mCamera.SetActive(active);
    }

    public void Explode()
    {
        ToggleView(true);
    }

    public void Reset()
    {
        ToggleView(false);
    }

    private void ToggleView(bool isExplosion)
    {
        tank.SetActive(!isExplosion);
        explosion.SetActive(isExplosion);
    }
}
