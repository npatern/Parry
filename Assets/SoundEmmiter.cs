using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmmiter : MonoBehaviour
{
    [SerializeField]
    private bool isEmiting = true;
    [SerializeField]
    private float soundRange = 20;
    [SerializeField]
    public ParticleSystem particle;
    private void FixedUpdate()
    {
      
        if (!isEmiting) return;
        Sound sound = new Sound(GetComponent<StatusController>(), soundRange, Sound.TYPES.cover);
        Sounds.MakeSound(sound);
        particle.startLifetime = soundRange / particle.startSpeed;
    }
   
    public void SetEmiting(bool isPowered)
    {
        isEmiting = isPowered;
        if (isPowered)
            particle.Play();
        else
            particle.Stop();
    }

}
