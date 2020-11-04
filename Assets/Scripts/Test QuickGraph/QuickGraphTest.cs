using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using QuickGraph;
using QuickGraph.Algorithms;

public class QuickGraphTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Create graph
        var edges = new Edge<int>[]
        {
            new Edge<int>(0, 1),
            new Edge<int>(1, 2),
            new Edge<int>(1, 3),
            new Edge<int>(2, 3),
            new Edge<int>(0, 2)


        };
        graph = edges.ToAdjacencyGraph<int, Edge<int>>();

        Func<Edge<int>, double> distances = x => 1.0;

        // Find shortest path
        var source = 0;
        var target = 2;

        var tryGetPath = graph.ShortestPathsDijkstra(distances, source);
        IEnumerable<Edge<int>> path;
        if (tryGetPath(target, out path))
        {
            foreach (var e in path)
            {
                Debug.LogFormat("{0}: {1}", GetType().Name, e);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AdjacencyGraph<int, Edge<int>> graph = null;
}
