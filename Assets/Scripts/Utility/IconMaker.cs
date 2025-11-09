using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class IconMaker : MonoBehaviour
{
    public Camera iconCamera;
    public static IconMaker Instance;

    private void Awake()
    {
        Instance = this;
    }

}