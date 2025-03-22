using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound
{
    public readonly float range;
    public readonly Vector3 position;
    public readonly StatusController statusController = null;
    public readonly StatusController EntityInfo = null;
    public readonly Vector3 PositionInfo = Vector3.zero;
    public enum TYPES { neutral, player, danger, cover, continous}
    public TYPES type;
    public Sound (StatusController _status, float _range, Vector3 _enemyPosition, TYPES _type = TYPES.neutral)
    {
        range = _range;

        position = _status.transform.position;
        statusController = _status;
        //worldInfo = _targetStatus;
        PositionInfo = _enemyPosition;
        type = _type;
        if (_status.IsDeaf() && type != TYPES.cover) range = 0;
    }

    public Sound(StatusController _status, float _range, TYPES _type = TYPES.neutral, StatusController _targetStatus = null)
    {
        range = _range;
        position = _status.transform.position;
        statusController = _status;
        EntityInfo = _targetStatus;
        type = _type;
        if (_status.IsDeaf() && type != TYPES.cover) range = 0;
    }
}
