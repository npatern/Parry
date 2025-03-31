using UnityEngine;

[CreateAssetMenu(fileName = "EntityStats", menuName = "ScriptableObjects/EntityStats", order = 1)]
public class EntityValuesScriptableObject : ScriptableObject
{
    [Space(10), Header("Posture")]

    [Tooltip("How fast after interaction posture will start to regenerate (in seconds)")]
    public float PostureRegenerationDelayTime = 1;

    [Tooltip("How fast will posture bar regenerate (/s)")]
    public float PostureRegenerationSpeed = 1;

    [Tooltip("How long stun lasts (in seconds)")]
    public float PostureStunnedRegenerationTime = 1;

    [Tooltip("Particles played if entity loses posture")]
    public ParticleSystem PostureEffect;

    [Tooltip("Particles played if entity is stunned")]
    public ParticleSystem PostureLongEffect;

    [Space(10), Header("Slowmo")]

    [Tooltip("How long will slowmo last after parry (in seconds)")]
    public float SlowmoTime = 1;
    [Tooltip("How slowed down time will be after parry")]
    public float SlowmoValue = .5f;

    [Space(10), Header("Awareness")]
    [Tooltip("How long it takes to notice player in full light(in seconds)")]
    public float AwarenessUpTime = 1;
    [Tooltip("How long it takes for awareness to drop to 0 (in seconds)")]
    public float AwarenessDownTime = 1;
    [Tooltip("How long it takes for awareness to drop to 0 when is alerted (in seconds)")]
    public float AlertedAwarenessDownTime = 5;
    [Tooltip("How long it takes for awareness to fill up while player run in range (in seconds)")]
    public float WalkSoundAwarenessTime = 1;



    [Space(10), Header("NPCspeed")]
    [Tooltip("How long it takes to notice player in full light(in seconds)")]
    public float NPCspeed = 4;
    [Tooltip("How long it takes for awareness to drop to 0 (in seconds)")]
    public float NPCspeedChase = 8;
}