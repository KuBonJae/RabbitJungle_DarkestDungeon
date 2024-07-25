using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorData : MonoBehaviour
{
    private Vector3 connectedDoorPosition;

    public Vector3 ConnectedDoorPosition
    {
        get { return connectedDoorPosition; }
    }

    public void Awake()
    {
        if (gameObject.name == "RightDoor")
        {
            connectedDoorPosition = new Vector3(transform.position.x + 41, transform.position.y, transform.position.z);
        }
        else if (gameObject.name == "LeftDoor")
        {
            connectedDoorPosition = new Vector3(transform.position.x - 41, transform.position.y, transform.position.z);
        }
        else if (gameObject.name == "TopDoor")
        {
            connectedDoorPosition = new Vector3(transform.position.x, transform.position.y + 42, transform.position.z);
        }
        else if (gameObject.name == "BottomDoor")
        {
            connectedDoorPosition = new Vector3(transform.position.x, transform.position.y - 42, transform.position.z);
        }
        if (gameObject.name == "SecretDoorEnter")
        {
            connectedDoorPosition = new Vector3(transform.position.x, transform.position.y + 42, transform.position.z);
        }
        else if (gameObject.name == "SecretDoorExit")
        {
            connectedDoorPosition = new Vector3(transform.position.x, transform.position.y - 42, transform.position.z);
        }
    }
}
