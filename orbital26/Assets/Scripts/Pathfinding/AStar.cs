using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 

public class AStar : MonoBehaviour
{
    public static AStar instance;
    private Node[] nodes;




    private void Awake()
    {
        //TOD: singleton pattern
        instance = this;
        nodes = FindObjectsOfType<Node>();
    }

    // find closest movement node to given position
    public Node GetNearestNode(Vector2 pos)
    {
        Node res = null;
        float best = float.MaxValue;
        foreach(Node n in nodes)
        {
            float d = Vector2.Distance(pos, n.transform.position);
            if (d < best)
            {
                best = d;
                res = n;
            }
        }
        return res;
    }

    // for random movements 
    public Node GetRandomNode()
    {
        int idx = Random.Range(0, nodes.Length);
        return nodes[idx];
    }

    // to search for specifc  nodes 
    // like entrance, exit, counter, ect
    public Node GetNodeByTag(string tag)
    {
        return System.Array.Find(nodes, x => x.CompareTag(tag));
    }

    // Generate path using nodes only then the unit use this to travel
    public List<Node> GeneratePath(Vector2 from, Vector2 to)
    {
        Node start = GetNearestNode(from);
        Node end = GetNearestNode(to);
        if (start == null || end == null)
        {
            Debug.Log(start + " " + end);
            return null;
        }

        return GeneratePath(GetNearestNode(from), GetNearestNode(to));
    }

    public List<Node> GeneratePath(Node start, Node end)
    {
        List<Node> open = new List<Node>();
        foreach(Node n in nodes)
        {
            n.gScore = float.MaxValue;
        }
        start.gScore = 0;
        start.hScore = Vector2.Distance(start.transform.position, end.transform.position);
        open.Add(start);
        while(open.Count > 0)
        {
            int lowerestF = default;
            for(int i = 1; i < open.Count; i++)
            {
                if (open[i].FScore() < open[lowerestF].FScore())
                {
                    lowerestF = i;
                }
            }
            Node curr = open[lowerestF];
            open.Remove(curr);

            if (curr == end)
            {
                List<Node> path = new List<Node>(); 
                while(curr != start)
                {
                    path.Add(curr);
                    curr = curr.cameFrom;
                }
                path.Reverse();
                return path;
            }
            foreach(Node v in curr.connections)
            {
                float held = curr.gScore + Vector2.Distance(curr.transform.position, v.transform.position);
                if (held < v.gScore)
                {
                    v.cameFrom = curr;
                    v.gScore = held;
                    v.hScore = Vector2.Distance(v.transform.position, end.transform.position);
                    if (!open.Contains(v))
                    {
                        open.Add(v);
                    }
                }
            }
        }
        // no path
        return null;
    }
}
