#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Linq;
public class NavMeshAutoBaker
{
    [MenuItem("Tools/NavMesh/Build All Surfaces %&n")] // Ctrl+Shift+B
    public static void BuildAllSurfaces()
    {
        var surfaces = Object.FindObjectsOfType<NavMeshSurface>();

        if (surfaces.Length == 0)
        {
            Debug.LogWarning("No NavMeshSurfaces found in scene.");
            return;
        }

        foreach (var surface in surfaces.OrderBy(s => s.agentTypeID))
        {
            Debug.Log($"Baking NavMeshSurface: {surface.name} (Agent Type ID: {surface.agentTypeID})");
            surface.BuildNavMesh();
        }

        Debug.Log($"Baked {surfaces.Length} NavMeshSurface(s).");
    }
}
#endif