using System;
using System.Collections.Generic;
using UnityEngine;

namespace CC.Stats
{
    /// <summary>
    /// A container for stats.
    /// Each stat is mapped to a unique enum value.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public class StatContainer<E> where E : System.Enum
    {
        /// <summary>
        /// Internal struct used to tick stats and for inspector display.
        /// </summary>
        [Serializable]
        protected struct StatWrapper
        {
            [CC.Stats.DisplayOnly] public E Name;
            public Stat Stat;
            public bool Tick;
            public StatWrapper(E name, Stat stat, bool tick)
            {
                this.Name = name;
                this.Stat = stat;
                this.Tick = tick;
            }
        }

        protected Dictionary<E, Stat> _stats = new Dictionary<E, Stat>();
        [SerializeField] protected List<StatWrapper> _statList = new List<StatWrapper>();

        /// <summary>
        /// Tick stats to enable change monitoring.
        /// </summary>
        public virtual void Tick()
        {
            for (int i = 0; i < _statList.Count; i++)
            {
                StatWrapper statWrapper = _statList[i];
            }
        }

        /// <summary>
        /// Initialize stat collections.
        /// </summary>
        public virtual void Clear()
        {
            _stats?.Clear();
            _statList?.Clear();
        }

        public virtual void Reset()
        {
        }

        /// <summary>
        /// Get a stat by enum value.
        /// </summary>
        /// <param name="statType"></param>
        /// <returns></returns>
        public virtual Stat Get(E statType)
        {
            if (_stats.TryGetValue(statType, out Stat stat))
            {
                return stat;
            };
            return null;
        }

        /// <summary>
        /// Get a stat by enum value.
        /// </summary>
        /// <param name="statType"></param>
        /// <param name="stat"></param>
        /// <returns></returns>
        public virtual bool TryGet(E statType, out Stat stat)
        {
            return _stats.TryGetValue(statType, out stat);
        }

        /// <summary>
        /// Map a stat to an enum value.
        /// Creates a new stat instance if it does not already exist.
        /// </summary>
        /// <param name="statType"></param>
        /// <param name="value"></param>
        /// <param name="tick"></param>
        /// <param name="upperBound"></param>
        /// <param name="lowerBound"></param>
        /// <returns></returns>
        public virtual Stat Create(E statType, float value, bool tick = true)
        {
            Stat stat = null;
            if (TryGet(statType, out stat))
            {
                return stat;
            }
            else
            {
                // Create new stat.
                stat = new Stat(0f);
                AddStat(statType, stat, tick);
            }
            return stat;
        }

        protected virtual void AddStat(E statType, Stat stat, bool observe = true)
        {
            _stats.Add(statType, stat);
            _statList.Add(new StatWrapper(statType, stat, observe));
        }
    }
}