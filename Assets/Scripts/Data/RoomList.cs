using System.Collections.Generic;

[System.Serializable]
public class Room
{
    public long id;
    public string name;
    public int maxPlayer;
    public int size;
}

public class RoomList
{
    public List<Room> rooms;
}
