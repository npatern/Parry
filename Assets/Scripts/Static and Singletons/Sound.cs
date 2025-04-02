using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound
{
    public readonly float range;
    public readonly Vector3 callerPosition;
    public readonly StatusController callerstatusController = null;
    public readonly StatusController targetEntityInfo = null;
    public readonly Vector3 targetPositionInfo = Vector3.zero;
    public readonly bool hasTargetInfo = false;
    public enum TYPES { neutral, player, danger, cover, continous}
    public TYPES type;
    public Sound (StatusController _status, float _range, Vector3 _enemyPosition, TYPES _type = TYPES.neutral)
    {
        range = _range;

        callerPosition = _status.transform.position;
        callerstatusController = _status;
        hasTargetInfo = true;
        targetPositionInfo = _enemyPosition;
        type = _type;
        if (_status.IsDeaf() && type != TYPES.cover) range = 0;
    }

    public Sound(StatusController _status, float _range, TYPES _type = TYPES.neutral, StatusController _targetStatus = null)
    {
        range = _range;
        callerPosition = _status.transform.position;
        callerstatusController = _status;
        targetEntityInfo = _targetStatus;
        hasTargetInfo = false;
        type = _type;
        if (_targetStatus != null)
        {
            hasTargetInfo = true;
            targetPositionInfo = _targetStatus.transform.position;
        }
            
        if (_status.IsDeaf() && type != TYPES.cover) range = 0;
    }
    public Sound(Vector3 _position, float _range, TYPES _type = TYPES.neutral)
    {
        range = _range;
        callerPosition = _position;
        callerstatusController = null;
        hasTargetInfo = false;
        targetEntityInfo = null;
        type = _type;
    }
}
