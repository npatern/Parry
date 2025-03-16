using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] float doorOpenTime = 1;
    [SerializeField] float doorCloseTime = 1;
    [SerializeField] Transform doorOpenPosition;
    [SerializeField] Transform door;
    Vector3 doorStartingPosition;
    Vector3 doorTargetPosition;
    bool DoorOpenState = false;

    float doorTimer = 0;

    public bool TICDOOR = false;

    private void Awake()
    {
        doorStartingPosition = door.transform.position;
        doorTargetPosition = door.transform.position;
    }
    private void Update()
    {
        if (TICDOOR) SwitchDoorState();
        doorTimer += Time.deltaTime;
        door.position = Vector3.Lerp(door.position, doorTargetPosition, doorTimer/doorCloseTime);
    }
    public void SwitchDoorState()
    {
        Debug.Log("DOOOR USEEEED state: "+ DoorOpenState);
        doorTimer = 0;
        DoorOpenState = !DoorOpenState;

        if (DoorOpenState) doorTargetPosition = doorOpenPosition.position;
        else doorTargetPosition = doorStartingPosition;

        TICDOOR = false;
       // return DoorOpenState;
    }
}
