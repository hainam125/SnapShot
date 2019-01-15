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

    private Transform nameCanvas;

    private void Start()
    {
        nameCanvas = nameTxt.transform.parent;
    }
    private void Update()
    {
        nameCanvas.LookAt(GameManager.Instance.CamTransform);
    }

    public void SetName(string name)
    {
        nameTxt.transform.parent.gameObject.SetActive(true);
        nameTxt.text = name;
    }

    public void UpdateHp(float hpPercentage)
    {
        hpRect.value = hpPercentage * 100f;
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
