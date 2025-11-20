using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveController : BaseController
{
    public static ObjectiveController _instance;
    public static ObjectiveController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ObjectiveController>();
            }

            return _instance;
        }
    }
    protected override void OnInitialize()
    {
        base.OnInitialize();
    }
    public ObjectiveScriptable defaultObjective;
    [SerializeReference]
    public ObjectiveWrapper currentObjective = null;
    private void Start()
    {
        if (currentObjective == null) currentObjective = new ObjectiveWrapper(defaultObjective);
    }
    private void Update()
    {
        if (currentObjective.CheckIfCompleted()) currentObjective = null;
        if (currentObjective == null) currentObjective = new ObjectiveWrapper(defaultObjective);
    }
}
