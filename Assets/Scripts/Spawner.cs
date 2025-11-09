using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public List<GameObject> spawnedObjects;
    public void SpawnObject()
    {
        SpawnObjectAndReturn(objectToSpawn);
    }
    public GameObject SpawnObjectAndReturn()
    {
        if (objectToSpawn == null) return null;
        return SpawnObjectAndReturn(objectToSpawn);
    }
    public GameObject SpawnObjectAndReturn(GameObject _gameObject)
    {
        if (_gameObject == null) return null;
        GameObject _obj = Instantiate(_gameObject, transform.position, transform.rotation);
        spawnedObjects.Add(_obj);
        return _obj;
    }
    public void KillSpawnedObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
                if (obj.TryGetComponent<StatusController>(out StatusController status))
                    status.Kill();
                else
                    Destroy(obj);
        }
        spawnedObjects.Clear();
    }
    public void RemoveSpawnedObjects()
    {
        for (int i=0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] == null)
            {
                spawnedObjects.RemoveAt(i);
                i -= 1;
                continue;
            }
            GameObject obj = spawnedObjects[i];
            Destroy(obj);
            spawnedObjects.RemoveAt(i);
        }
        spawnedObjects.Clear();
    }
}
