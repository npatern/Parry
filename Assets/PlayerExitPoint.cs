using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExitPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<StatusController>()== null) return;
        if (other.GetComponent<StatusController>().IsPlayer)
            LevelController.Instance.SetExitReached(true);
    }
}
