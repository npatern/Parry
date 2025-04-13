using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerNPC : MonoBehaviour
{
    public GameObject npc;
    public DisguiseScriptable disguise;
    public int nrToSpawn;
    public void RunSpawner()
    {
        for (int i = 0; i < nrToSpawn; i++)
        {
            Instantiate(npc, transform.position, transform.rotation, GameController.Instance.EntitiesParent);
            Debug.Log("!!!!!!!!!!!!!!!!!soawbnef");
        }
    }
}
