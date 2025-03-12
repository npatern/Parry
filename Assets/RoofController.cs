using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofController : MonoBehaviour
{
    Transform playerPosition;
    GameController gameController;
    MeshRenderer mesh;
    public float minDistance = 10;
    void Start()
    {
        gameController = GameController.Instance;
        
 
        mesh = this.GetComponent<MeshRenderer>();
    }

    void Update()
    {
       
        if (mesh == null) return;
        if (playerPosition == null) playerPosition = gameController.CurrentPlayer.transform;
        mesh.enabled = !IsConflictingWithPlayer();
    }
    bool IsConflictingWithPlayer()
    {
        if (Vector3.Distance(playerPosition.position, transform.position) < minDistance)
            if (playerPosition.position.y < transform.position.y)
                return true;

        return false;
    }
}
