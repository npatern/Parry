using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public ZoneScriptable zone;
    public ZoneWrapper zoneWrapper;
    [Tooltip("Zone with highest nr will affect player")]
    public int Depth = 1;
}
