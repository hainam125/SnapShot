using NetworkMessage;
using UnityEngine;

public class ClientInput : MonoBehaviour
{
    private ClientSound sound;

    private bool up;
    private bool down;
    private bool left;
    private bool right;
    private bool fire;

    private float fireRate;
    private float timeNextFire;

    private bool pressUp;
    private bool pressDown;
    private bool pressLeft;
    private bool pressRight;
    private bool pressFire;

    public void ToggleDown(bool r) { pressDown = r; }
    public void ToggleUp(bool r) { pressUp = r; }
    public void ToggleLeft(bool r) { pressLeft = r; }
    public void ToggleRight(bool r) { pressRight = r; }
    public void ToggleUpRight(bool r) { pressUp = r; pressRight = r; }
    public void ToggleUpLeft(bool r) { pressUp = r; pressLeft = r; }
    public void ToggleDownRight(bool r) { pressDown = r; pressRight = r; }
    public void ToggleDownLeft(bool r) { pressDown = r; pressLeft = r; }
    public void ToggleFire(bool r) { pressFire = r; }

    private void Awake()
    {
        sound = GetComponent<ClientSound>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W) || pressUp)
        {
            up = true;
        }
        else if (Input.GetKey(KeyCode.S) || pressDown)
        {
            down = true;
        }
        if (Input.GetKey(KeyCode.D) || pressRight)
        {
            right = true;
        }
        else if (Input.GetKey(KeyCode.A) || pressLeft)
        {
            left = true;
        }
        if (Input.GetKey(KeyCode.Space) || pressFire)
        {
            if (timeNextFire < Time.timeSinceLevelLoad)
            {
                fire = true;
                timeNextFire = Time.timeSinceLevelLoad + 1f / fireRate;
                sound.ShotFire();
            }
        }
    }

    public byte GetCmd()
    {
        byte cmd = 0;
        if (up) cmd |= Command.Keys[KeyCode.W];
        else if (down) cmd |= Command.Keys[KeyCode.S];
        if (right) cmd |= Command.Keys[KeyCode.D];
        else if (left) cmd |= Command.Keys[KeyCode.A];
        if (fire) cmd |= Command.Keys[KeyCode.Space];

        if (cmd != 0)
        {
            up = false; down = false; right = false; left = false; fire = false;
        }
        return cmd;
    }

    public void SetFireRate(float rateOfFrie)
    {
        fireRate = rateOfFrie;
    }
}
