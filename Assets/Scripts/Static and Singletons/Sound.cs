using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound
{
    public readonly float range;
    public readonly Vector3 position;
    public readonly Transform worldInfo;
    public enum TYPES { neutral, player, danger, cover}
    public TYPES type;
    public Sound (Vector3 _position, float _range, TYPES _type = TYPES.neutral, Transform _worldInfo = null)
    {
        range = _range;
        position = _position;
        worldInfo = _worldInfo;
        type = _type;
    }
    public Sound(StatusController _status, float _range, TYPES _type = TYPES.neutral, Transform _worldInfo = null)
    {

        range = _range;
        position = _status.transform.position;
        
        worldInfo = _worldInfo;
        type = _type;
        if (_status.IsDeaf() && type != TYPES.cover) range = 0;
    }
}
