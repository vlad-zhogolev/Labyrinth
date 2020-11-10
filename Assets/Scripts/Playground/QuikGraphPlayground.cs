using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuikGraph.Algorithms;
using QuikGraph;

namespace Playground {

    using Vertex = Labyrinth.Vertex;
    using MyEdge = QuikGraph.EquatableUndirectedEdge<Labyrinth.Vertex>;

    //using MyEdge = QuikGraph.EquatableUndirectedEdge<Vertex>;

    //using Vertex = MyVecVertex;
    //using MyEdge = QuikGraph.EquatableUndirectedEdge<MyVecVertex>;

    struct MyVertex : IEquatable<MyVertex>, IComparable<MyVertex>
    {
        public MyVertex(int a, int b) { x = a ; y = b;}
        public int x;
        public int y;

        public int CompareTo(MyVertex other)
        {
            if (x == other.x && y == other.y)
            {
                return 0;
            }

            if (x < other.x)
            {
                return -1;
            }
            else if (x == other.x)
            {
                if (y < other.y)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 1;
            }
        }

        public bool Equals(MyVertex other)
        {
            return (x == other.x) && (y == other.y);
        }
    }

    struct MyVecVertex : IEquatable<MyVecVertex>, IComparable<MyVecVertex>
    {
        public MyVecVertex(int a, int b)
        {
            indices = new Vector2Int(a, b);
        }

        public Vector2Int indices;

        public int CompareTo(MyVecVertex other)
        {
            if (indices.x == other.indices.x && indices.y == other.indices.y)
            {
                return 0;
            }

            if (indices.x < other.indices.x)
            {
                return -1;
            }
            else if (indices.x == other.indices.x)
            {
                if (indices.y < other.indices.y)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 1;
            }
        }

        public bool Equals(MyVecVertex other)
        {
            return (indices.x == other.indices.x) && (indices.y == other.indices.y);
        }
    }

public class QuikGraphPlayground : MonoBehaviour
{
    
    static MyEdge CreateEdge(Vertex x, Vertex y)
    {
        if (x.CompareTo(y) > 0)
        {
            return new MyEdge(y, x);
        }
        else
        {
            return new MyEdge(x, y);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        var edges = new MyEdge[]
        {
            CreateEdge(new Vertex(1, 0), new Vertex(0, 0)),
            CreateEdge(new Vertex(1, 0), new Vertex(0, 0)),
            CreateEdge(new Vertex(2, 1), new Vertex(1, 0)),
            CreateEdge(new Vertex(3, 2), new Vertex(2, 1)),
            CreateEdge(new Vertex(2, 1), new Vertex(2, 2))
            //new MyEdge(new Vertex(1, 0), new Vertex(0, 0))
        };

        var graph = new UndirectedGraph<Vertex, MyEdge>(false, (e, source, target) => {return e.Equals(CreateEdge(source, target));});
        graph.AddVerticesAndEdgeRange(edges);


        //var graph = edges.ToUndirectedGraph<Vertex, MyEdge>(false);

        var edge1 = CreateEdge(new Vertex(1, 0), new Vertex(0, 0));
        var result1 = graph.ContainsEdge(edge1);
        var edge2 = CreateEdge(new Vertex(0, 0), new Vertex(1, 0));
        var equality1 = edge1.Equals(edge2);
        var result2 = graph.ContainsEdge(edge2);
        var result3 = graph.ContainsEdge(new Vertex(0, 0), new Vertex(1, 0));
        var result4 = graph.ContainsEdge(new Vertex(1, 0), new Vertex(0, 0));

        var equality2 = new Vector2Int(1, 0).Equals(new Vector2Int(1, 0));
        var equality3 = edge1.Equals(edge2);
        var comparison1 = edge1.Source.CompareTo(edge2.Source);
        var edge = CreateEdge(new Vertex(2, 1), new Vertex(3, 2));

        var equality4 = graph.EdgeEqualityComparer(edge1, new Vertex(0, 0), new Vertex(1, 0));
        var equality5 = graph.EdgeEqualityComparer(edge1, new Vertex(1, 0), new Vertex(0, 0));

        //var adjacent1 = graph.AdjacentEdges(new Vertex(0, 0));
        var adjacent2 = graph.AdjacentEdges(new Vertex(1, 0));

        var result = graph.RemoveEdge(edge);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

}
