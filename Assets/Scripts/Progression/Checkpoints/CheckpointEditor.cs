#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class CheckpointEditor : MonoBehaviour
{
    [SerializeField] private GameObject checkpointPrefab;
    
    public void AddCheckpoint(Vector3 spawnPosition)
    {
        GameObject clone = PrefabUtility.InstantiatePrefab(checkpointPrefab) as GameObject;
        clone.transform.position = spawnPosition;
        clone.transform.SetParent(transform, true);
    }

    private void OnDrawGizmos()
    {
        var cps = gameObject.GetComponentsInChildren<Checkpoint>();

        for (int i = 0; i < cps.Length - 1; i++)
        {
            Gizmos.DrawLine(cps[i].transform.position, cps[i+1].transform.position);
        }
    }
}


// Sergei: If you know of a better naming convention then 'EditorEditor' - let me know.

[CustomEditor(typeof(CheckpointEditor))]
public class CheckpointEditorEditor : Editor 
{
    private Vector3 spawnPosition;
    
    private void OnSceneGUI()
    {
        spawnPosition = SceneView.currentDrawingSceneView.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Add Checkpoint"))
        {
            var manager = (CheckpointEditor)target;
            manager.AddCheckpoint(spawnPosition);
        }
    }
}

#endif
