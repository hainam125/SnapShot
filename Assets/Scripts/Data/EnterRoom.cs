using System.Collections.Generic;

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
