using System;
using System.Collections.Generic;

namespace CC.Stats
{
    /// <summary>
    /// A container for stats.
    /// Each stat is mapped to a unique enum value.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public class StatContainer<E> where E : Enum
    {
        private Dictionary<E, Stat> _stats = new Dictionary<E, Stat>();

        public void Add(E e, Stat stat)
        {
            _stats[e] = stat;
        }

        public void Clear()
        {
            _stats.Clear();
        }
    }
}