using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofController : MonoBehaviour
{
    Transform playerPosition;
    GameplayController gameController;
    MeshRenderer mesh;
    public float minDistance = 10;
    public Transform target;
    public Vector3 offset = Vector3.up / 2;
    void Start()
    {
        gameController = GameplayController.Instance;
        
 
        mesh = this.GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (gameController.CurrentPlayer == null) return;
        if (playerPosition == null) playerPosition = gameController.CurrentPlayer.transform;
        if (mesh == null)
            target.gameObject.SetActive(!IsConflictingWithPlayer());
        else
            mesh.enabled = !IsConflictingWithPlayer();
    }
    bool IsConflictingWithPlayer()
    {
        if (Vector3.Distance(playerPosition.position, transform.position) < minDistance)
            if (playerPosition.position.y+ offset.y < transform.position.y)
                return true;

        return false;
    }
}
