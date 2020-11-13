using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using QuickGraph;
using QuickGraph.Algorithms;

namespace Playground { 
public class QuickGraphPlayground : MonoBehaviour
{
    class Dummy
    {
        public Dummy(int n)
        {
            num = n;
        }
        public int num;
    }

    class MyUndirectedEdge<TVertex> : UndirectedEdge<TVertex>, IEquatable<MyUndirectedEdge<TVertex>>
    {
        public MyUndirectedEdge(TVertex source, TVertex target) : base(source, target) {}

        public bool Equals(MyUndirectedEdge<TVertex> other)
        {
            if (this == other)
            {
                return true;
            }

            return (Source.Equals(other.Source) && Target.Equals(other.Target)) ||
                   (Source.Equals(other.Target) && Target.Equals(other.Source));
        }
    }

    void DictinaryTest()
    {
       var dict = new  Dictionary<Dummy, UndirectedEdge<Dummy>>();
       dict.Add(new Dummy(0), new MyUndirectedEdge<Dummy>(new Dummy(0), new Dummy(1)));

       var first = new Dummy(0);
       var second = new Dummy(1);

       var list = new List<MyUndirectedEdge<Dummy>>();
       list.Add(new MyUndirectedEdge<Dummy>(first, second));

       list.Remove(new MyUndirectedEdge<Dummy>(first, second));

       dict.Remove(new Dummy(0));


       var stringDict = new Dictionary<string, Dummy>();
       stringDict.Add("hell", new Dummy(0));
       stringDict.Remove("hell");

    }

    void IntTest()
    {
        QuickGraph.UndirectedGraph<int, QuickGraph.SEquatableUndirectedEdge<int>> m_graph =
            new QuickGraph.UndirectedGraph<int, QuickGraph.SEquatableUndirectedEdge<int>>(
                false,
                (edge, source, target) => {
                    return (edge.Source.Equals(source) && edge.Target.Equals(target)) ||
                           (edge.Source.Equals(target) && edge.Target.Equals(source));
                }
            );

        
    }

   

    // Start is called before the first frame update
    void Start()
    {
        DictinaryTest();
        var edge = new UndirectedEdge<int>(1, 3);
        // Create graph
        var edges = new UndirectedEdge<int>[]
        {
            new UndirectedEdge<int>(0, 1),
            new UndirectedEdge<int>(1, 2),
            edge,
            new UndirectedEdge<int>(3, 1),
            //new Edge<int>(2, 3),
            new UndirectedEdge<int>(0, 2)

            
        };

        

            //graph = edges.ToAdjacencyGraph<int, Edge<int>>();
        graph = edges.ToUndirectedGraph<int, UndirectedEdge<int>>();

        //var comparer = graph.EdgeEqualityComparer;
        // var re2 = comparer(edge, 1, 3);
        //var res1 = graph.RemoveEdge(new UndirectedEdge<int>(1, 2));
        UndirectedEdge<int> edgeres;
        //var result = graph.TryGetEdge(2, 1, out edgeres);
        //var res = graph.RemoveEdge(edgeres);
        var compar1 = Comparer<int>.Default.Compare(2, 1);
        var compar2 = Comparer<int>.Default.Compare(1, 2);

        Func<UndirectedEdge<int>, double> distances = x => 1.0;

        // Find shortest path
        var source = 3;
        var target = 2;

        var tryGetPath = graph.ShortestPathsDijkstra(distances, source);
        IEnumerable<UndirectedEdge<int>> path;
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

    public UndirectedGraph<int, UndirectedEdge<int>> graph = null;
}

} // namespace QuickGraphTest