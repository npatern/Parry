using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : BaseController
{
    public static CameraController _instance;
    public static CameraController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<CameraController>();
            }

            return _instance;
        }
    }
    protected override void OnInitialize()
    {
        base.OnInitialize();
    }
}
