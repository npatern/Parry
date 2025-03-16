using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Sounds 
{
    public static void MakeSound(Sound sound)
    {
        if (sound.range <= 0) return;
        //Gizmos.DrawSphere(sound.position, sound.range);
        Debug.DrawRay(sound.position+Vector3.forward,Vector3.up* sound.range  + Vector3.up);

        Collider[] colliders = Physics.OverlapSphere(sound.position, sound.range);
        foreach (Collider col in colliders)
            if (col.TryGetComponent(out IHear hearer))
            {
                IHear[] hearers = col.GetComponents<IHear>();
                for (int i =0; i<hearers.Length; i++)
                {
                    hearers[i].ReactToSound(sound);
                }  
            }
    }
}
