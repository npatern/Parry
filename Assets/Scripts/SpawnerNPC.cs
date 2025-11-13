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
            OutwardController nakedGuy = Instantiate(npc, transform.position, transform.rotation, LevelController.Instance.EntitiesParent).GetComponent<OutwardController>();
            nakedGuy.WearDisguise(disguise);
            nakedGuy.GetComponent<EntityController>().target = LevelController.Instance.CurrentPlayer;
            if (disguise.item != null)
                nakedGuy.GetComponent<ToolsController>().EquipItem(new ItemWeaponWrapper(disguise.item));
        }
    }
}
