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
    bool DoorOpenState = true;

    float doorTimer = 0;

    public bool ForceChange = false;

    private void Awake()
    {
        doorStartingPosition = door.transform.position;
        doorTargetPosition = door.transform.position;
        SwitchDoorState(DoorOpenState);
    }
    private void Update()
    {
        if (ForceChange) SwitchDoorState(); ForceChange = false;
        doorTimer += Time.deltaTime;
        door.position = Vector3.Lerp(door.position, doorTargetPosition, doorTimer / doorCloseTime);
    }
    public void SwitchDoorState(bool TagetOpenState)
    {
        //if (DoorOpenState == TagetOpenState) return;
        doorTimer = 0;
        DoorOpenState = TagetOpenState;

        if (DoorOpenState) doorTargetPosition = doorOpenPosition.position;
        else doorTargetPosition = doorStartingPosition;
    }
    public void SwitchDoorState()
    {
        SwitchDoorState(!DoorOpenState);
    }
}
