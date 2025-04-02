using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Stats ", menuName = "ScriptableObjects/Stats/Stats", order = 2)]
public class StatsScriptable : ScriptableObject
{
    [SerializeField]
    public Stat[] stats;
}