using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Navigator : MonoBehaviour
{

    public Node startNode;
    public Node endNode;
    Node currentNode;

    public List<Node> path;

    public List<Node> unvisitedNodes = new List<Node>();
    public List<Node> visitedNodes = new List<Node>();

    public List<Node> CalculatePath(Node startNode, Node endNode)
    {
        this.startNode = startNode;
        this.endNode = endNode;
        Calculate();
        return path;
    }

    void Calculate()
    {
        //setup
        startNode.distance = 0;
        unvisitedNodes.Add(startNode);
        Debug.Log("make path from " + startNode + " to " + endNode);
        //loop
        while (unvisitedNodes.Count > 0 && path.Count == 0)
        {
            currentNode = unvisitedNodes[0];
            if (currentNode == endNode)
            {
                visitedNodes.Add(currentNode);
                unvisitedNodes.Remove(currentNode);
                continue;
            }
            List<NodeTuple> neighbours = currentNode.neighbours;
            foreach (NodeTuple t in neighbours)
            {
                if (visitedNodes.Contains(t.node))
                    continue;
                int dist = currentNode.distance + t.weight;
                if (t.node.distance > dist)
                {
                    t.node.distance = dist;
                    t.node.previusNode = currentNode;
                }
                if (!unvisitedNodes.Contains(t.node))
                    unvisitedNodes.Add(t.node);
            }
            visitedNodes.Add(currentNode);
            unvisitedNodes.Remove(currentNode);
            unvisitedNodes.OrderBy(n => n.GetDistance(endNode) + n.distance);
        }
        MakePath();
        ResetNodes();
    }
    void MakePath()
    {
        currentNode = endNode;
        while (currentNode.previusNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.previusNode;
        }
    }
    void ResetNodes()
    {
        while (visitedNodes.Count != 0)
        {
            visitedNodes[0].previusNode = null;
            visitedNodes[0].distance = 99999;

            visitedNodes.RemoveAt(0);

        }
    }
}
