using NetworkMessage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterRoom
{
    public string name;
    public long objectId;
    public string snapShot;
    public List<long> ids;
    public List<string> usernames;

    public EnterRoom(string _name)
    {
        name = _name;
    }
}
