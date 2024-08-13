using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FramerateDisplay : MonoBehaviour
{
    TextMeshProUGUI textMesh = null;
    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }
    int framerate = 0;
    void Update()
    {
        framerate = (int)(1 / Time.deltaTime);

        if (textMesh)
        {
            textMesh.text = framerate.ToString();
        }
    }
}
