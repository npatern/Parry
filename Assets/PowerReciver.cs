using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerReciver : MonoBehaviour
{
    // Gravity simulation parameters
    public int segments = 20; // Number of segments to approximate the curve
    public float gravityStrength = 2f; // Adjust this value to control the sag


    public bool NeedsPowerSource = false;
    public bool PowerComingIn = false;
    public bool IsSwitchedOn = true;
    [SerializeField]
    protected PowerNode powerSource;

    public bool ShowWires = true;
    private LineRenderer Wires;
    protected virtual void Awake()
    {
        if (powerSource != null)
        {
            NeedsPowerSource = true;
        }
        else
        {
            NeedsPowerSource = false;
        }
    }
    protected virtual void Start()
    {
        if (powerSource!= null)
        {
            
            powerSource.PowerChangedEvent += OnPowerChanged;
            NeedsPowerSource = true;
        }
        OnPowerChanged(CheckIfPowered());
        StartCoroutine(DelayedRefresh());
    }

    protected IEnumerator DelayedRefresh()
    {
        yield return new WaitForFixedUpdate(); // lub yield return new WaitForSeconds(0.1f);
        yield return 1;
        LateStart();
    }

    protected virtual void LateStart()
    {
        if (NeedsPowerSource)
           // GenerateWiresB();
        GenerateWires();
    }
    protected virtual void OnPowerChanged(bool powered)
    {
        PowerComingIn = powered;
    }
    protected bool CheckIfPowered()
    {
        if (!NeedsPowerSource) return true;
        if (PowerComingIn) return true;
        return false;
    }
    protected virtual void OnDrawGizmos()
    {
        if (CheckIfPowered())
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.up + transform.right*-1, .3f);
    }
    void GenerateWires()
    {
        if (!ShowWires || powerSource == null) return;
        if (powerSource.gameObject == gameObject) return;
        if (Wires == null)
        Wires = gameObject.AddComponent<LineRenderer>();
        Wires.useWorldSpace = true;

        // Visual setup
        Wires.material = new Material(Shader.Find("Sprites/Default")); // Replace with your material if needed
        Wires.startColor = Color.black;
        Wires.endColor = Color.black;
        Wires.startWidth = 0.05f;
        Wires.endWidth = 0.05f;

        

        Vector3 startPoint = transform.position;
        Vector3 endPoint = powerSource.transform.position;

        Wires.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 point = Vector3.Lerp(startPoint, endPoint, t);

            // Apply a downward offset to simulate sagging
            float sag = Mathf.Sin(t * Mathf.PI) * gravityStrength;
            point.y -= sag;

            Wires.SetPosition(i, point);
        }
    }
    void GenerateWiresB()
    {
        if (!ShowWires || powerSource == null) return;
        if (powerSource.gameObject == gameObject) return;

        int cableCount = 5; // Number of cables to generate
        int segments = 20;  // Number of segments per cable for smoothness

        for (int i = 0; i < cableCount; i++)
        {
            // Create a new GameObject for each cable
            GameObject cableObj = new GameObject("Cable_" + i);
            cableObj.transform.parent = null;

            LineRenderer wire = cableObj.AddComponent<LineRenderer>();
            wire.useWorldSpace = true;

            // Visual setup
            wire.material = new Material(Shader.Find("Sprites/Default"));
            wire.startWidth = 0.05f;
            wire.endWidth = 0.05f;

            // Assign a color with slight variation
            Color baseColor = Color.Lerp(Color.black, Color.blue, i / (float)cableCount);
            wire.startColor = baseColor;
            wire.endColor = baseColor;

            wire.positionCount = segments + 1;

            Vector3 startPoint = transform.position;
            Vector3 endPoint = powerSource.transform.position;

            for (int j = 0; j <= segments; j++)
            {
                float t = j / (float)segments;
                Vector3 point = Vector3.Lerp(startPoint, endPoint, t);

                // Apply a downward offset to simulate sagging
                float sag = Mathf.Sin(t * Mathf.PI) * (gravityStrength+ Random.Range(-.5f, .5f));
                point.y -= sag;

                // Apply a slight horizontal offset to differentiate cables
                Vector3 perpendicular = Vector3.Cross((endPoint - startPoint).normalized, Vector3.up);
               
                point += perpendicular * Mathf.Sin(t * Mathf.PI) * Random.Range(-0.1f, 0.1f);

                wire.SetPosition(j, point);
            }
        }
    }
}
