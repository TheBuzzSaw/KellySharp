using System.Collections.Generic;

namespace KellySharp.ActorModel
{
    public class ActorSystem
    {
        private readonly List<KeyValuePair<int, Actor?>> _actors = new List<KeyValuePair<int, Actor?>>();
    }
}