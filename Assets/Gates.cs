using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Gates : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numOfGates;
    public List<Checkpoint> gates;
    private void Start()
    {
        numOfGates = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        gates.AddRange(Checkpoint.FindObjectsOfType<Checkpoint>());
        Debug.Log(gates.Count());

    }
    private IEnumerator gateCheck()
    {
        yield return new WaitForSeconds(.1f);
        if (gates.Count() != Checkpoint.FindObjectsOfType<Checkpoint>().Length)
        {
            gates.Clear();
            gates.AddRange(Checkpoint.FindObjectsOfType<Checkpoint>());
            Debug.Log(gates.Count());
        }
        numOfGates.text = gates.Count().ToString();
    }
    private void Update()
    {
        StartCoroutine(gateCheck());
    }
}
