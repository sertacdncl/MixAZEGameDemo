using System;
using System.Collections.Generic;

namespace MeshTools
{
	internal class Node : IEquatable<Node>
    {
        public string Name;
        public int Id { get; }
        public int Rank;
        public Border Border { get; }
        public HashSet<Node> Children = new HashSet<Node>();

        public Node(int id, Border border)
        {
            Id = id;
            Border = border;
        }

        public bool Equals(Node other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Node) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}