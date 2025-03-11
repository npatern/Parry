using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmmiter : MonoBehaviour
{
    public float soundRange = 20;
    public ParticleSystem particle;
    private void FixedUpdate()
    {
        Sound sound = new Sound(GetComponent<StatusController>(), soundRange, Sound.TYPES.cover);
        Sounds.MakeSound(sound);
        particle.startLifetime = soundRange / particle.startSpeed;
    }
    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawSphere(transform.position, soundRange);
    }
}
