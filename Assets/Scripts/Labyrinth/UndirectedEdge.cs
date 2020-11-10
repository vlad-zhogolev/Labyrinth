using System;

namespace Labyrinth {

public class UndirectedEdge<TVertex> : QuickGraph.UndirectedEdge<TVertex>, IEquatable<UndirectedEdge<TVertex>>
{
    public UndirectedEdge(TVertex source, TVertex target) : base(source, target) {}

    public bool Equals(UndirectedEdge<TVertex> other)
    {
        if (this == other)
        {
            return true;
        }

        return (Source.Equals(other.Source) && Target.Equals(other.Target)) ||
               (Source.Equals(other.Target) && Target.Equals(other.Source));
    }
}

} // namespace QuickGraphTest 
