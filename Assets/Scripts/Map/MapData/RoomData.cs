using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    private string roomType;
    private int remainedEnemy = 1;

    public string RoomType
    {
        get { return roomType; }
        set { roomType = value; }
    }

    public int RemainedEnemy
    {
        get { return remainedEnemy; }
        set { remainedEnemy = value; }
    }
}
