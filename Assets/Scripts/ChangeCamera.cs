using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    public Transform[] cameraPositions;
    public bool randomize = false;
    private void Start()
    {
        RandomCameraPosition();
    }
    private void Update()
    {
        if (randomize) RandomCameraPosition();
        randomize = false;
    }
    void RandomCameraPosition()
    {
        int index = Random.Range(0, cameraPositions.Length);
        transform.position = cameraPositions[index].position;
        transform.rotation = cameraPositions[index].rotation;
    }
}
