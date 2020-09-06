using System;
using System.Diagnostics.CodeAnalysis;

namespace KellySharp.ActorModel
{
    public readonly struct ActorReference : IEquatable<ActorReference>
    {
        internal readonly int Index { get; }
        internal readonly int Generation { get; }
        
        internal ActorReference(int index, int generation)
        {
            Index = index;
            Generation = generation;
        }

        public bool Equals(ActorReference other) => Index == other.Index && Generation == other.Generation;
        public override bool Equals(object? obj) => obj is ActorReference ar && Equals(ar);
        public override int GetHashCode() => HashCode.Combine(Index, Generation);

        // TODO: Obfuscate?
        public override string ToString() => $"{Index}-{Generation}";
    }
}