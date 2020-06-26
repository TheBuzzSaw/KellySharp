using System;
using System.Collections.Immutable;

namespace KellySharp
{
    public static class KeyFilter
    {
        public static KeyFilter<T> Allow<T>(params T[] keys)
        {
            return new KeyFilter<T>(ImmutableHashSet.Create(keys), true);
        }

        public static KeyFilter<T> Deny<T>(params T[] keys)
        {
            return new KeyFilter<T>(ImmutableHashSet.Create(keys), false);
        }
        
        public static KeyFilter<T> Combine<T>(KeyFilter<T> kf1, KeyFilter<T> kf2)
        {
            if (kf1.AllowKeys)
            {
                if (kf2.AllowKeys)
                {
                    return new KeyFilter<T>(kf1.Keys.Union(kf2.Keys), true);
                }
                else
                {
                    return new KeyFilter<T>(kf2.Keys.Except(kf1.Keys), false);
                }
            }
            else if (kf2.AllowKeys)
            {
                return new KeyFilter<T>(kf1.Keys.Except(kf2.Keys), false);
            }
            else
            {
                return new KeyFilter<T>(kf1.Keys.Intersect(kf2.Keys), false);
            }
        }
    }
    
    public class KeyFilter<T>
    {
        public ImmutableHashSet<T> Keys { get; }
        public bool AllowKeys { get; }
        public bool DenyKeys => !AllowKeys;
        
        public KeyFilter(ImmutableHashSet<T> keys, bool allowKeys)
        {
            if (keys is null)
                throw new ArgumentNullException(nameof(keys));
            
            Keys = keys;
            AllowKeys = allowKeys;
        }

        public bool Passes(T key) => Keys.Contains(key) == AllowKeys;

        public override string ToString()
        {
            var word = AllowKeys ? "allow " : "deny ";
            return word + string.Join(", ", Keys);
        }
    }
}