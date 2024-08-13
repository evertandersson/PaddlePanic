#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class StreamEditor : MonoBehaviour
{
    [SerializeField] private Mesh streamPointMesh;
    [SerializeField, Range(0f, 10f)] private float scale = 1f;

    public void AddStreamPoint(Vector3 spawnPosition)
    {
        var inst = new GameObject("StreamPoint");
        inst.transform.parent = transform;
        inst.transform.position = spawnPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for(int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            Gizmos.DrawMesh(streamPointMesh, child.transform.position, child.transform.rotation, scale * child.transform.localScale);
        }
    }

    private void Start()
    {
        //SendToInterpolator();
    }
}

[CustomEditor(typeof(StreamEditor))]
public class StreamEditorGUI : Editor
{
    private Vector3 spawnPosition;

    private void OnSceneGUI()
    {
        float dist = -Camera.current.transform.position.y / Camera.current.transform.forward.y;
        spawnPosition = SceneView.currentDrawingSceneView.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, dist));
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Add Stream Point"))
        {
            var manager = (StreamEditor)target;
            manager.AddStreamPoint(spawnPosition);
        }
    }
}

#endif