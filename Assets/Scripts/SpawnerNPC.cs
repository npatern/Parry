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
            GameplayController.Instance.SpawnNPC(transform, disguise);
            /*
            OutwardController nakedGuy = Instantiate(npc, transform.position, transform.rotation, GameplayController.Instance.EntitiesParent).GetComponent<OutwardController>();
            nakedGuy.WearDisguise(disguise);
            nakedGuy.GetComponent<EntityController>().target = GameplayController.Instance.CurrentPlayer;
            if (disguise.item != null)
                nakedGuy.GetComponent<ToolsController>().EquipItem(new ItemWeaponWrapper(disguise.item));
            */
        }
    }
}
