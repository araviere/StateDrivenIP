using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    // Start is called before the first frame update

    public List<NodeTuple> neighbours = new List<NodeTuple>();
    public Node previusNode;

    public int distance = 2000000;

    public (int number, string name) tupleStuff;
    public int GetDistance(MonoBehaviour node) { return (int)Vector3.Distance(this.transform.position, node.transform.position); }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        foreach(NodeTuple node in neighbours)
        {
            Gizmos.DrawLine(this.transform.position, node.node.transform.position);

        }
    }
}
