using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// guide to A*: https://www.youtube.com/watch?v=UHnOW-OimLQ&t=118s
public class Node : MonoBehaviour
{
    public Node cameFrom;
    public List<Node> connections;

    // heuristic to start and end
    // honestly just blackbox this algo its not important to our project
    // we can ignore the implementation details
    public float gScore;
    public float hScore;

    public float FScore()
    {
        return gScore + hScore;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (connections.Count > 0)
        {
            for(int i = 0; i < connections.Count; i++)
            {
                Gizmos.DrawLine(transform.position, connections[i].transform.position);
            }
        }
    }
}
