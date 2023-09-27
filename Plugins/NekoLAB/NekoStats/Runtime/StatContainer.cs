using System.Collections.Generic;
using UnityEngine;
using NekoLab.ReactiveProps;

namespace NekoLab.Stats
{
    /// <summary>
    /// A container for stats.
    /// Each stat is mapped to a unique enum value.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    [System.Serializable]
    public class StatContainer<E> where E : System.Enum
    {
        /// <summary>
        /// Internal struct used to tick stats and for inspector display.
        /// </summary>
        [System.Serializable]
        protected struct StatWrapper
        {
            [NekoLab.Stats.DisplayOnly] public E Name;
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
        /// Initialize stat collections.
        /// </summary>
        public virtual void Clear()
        {
            _stats?.Clear();
            _statList?.Clear();
        }

        /// <summary>
        /// Tick stats to enable change monitoring.
        /// </summary>
        public virtual void Tick()
        {
            for (int i = 0; i < _statList.Count; i++)
            {
                StatWrapper statWrapper = _statList[i];
                if (statWrapper.Tick) statWrapper.Stat.Tick();
            }
        }

        public virtual void ResetStats()
        {
            foreach (Stat stat in _stats.Values)
            {
                stat.Reset();
            }
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
        public virtual Stat RegisterStat(E statType, float value, bool tick = true)
        {
            Stat stat;
            if (TryGet(statType, out stat) == false)
            {
                // Create new stat.
                stat = new Stat(value);
                AddStat(statType, stat, tick);
            }
            else
            {
                stat.BaseValue = value;
                stat.InitialValue = value;
            }
            return stat;
        }

        /// <summary>
        /// Register a resource stat.
        /// <para>The upper bound is set to another stat corresponding the specified enum value.</para>
        /// <para>The lower bound is set to 0.</para>
        /// Make sure the upper bound stat is registered before this resource stat.
        /// </summary>
        /// <param name="statType"></param>
        /// <param name="value"></param>
        /// <param name="upperBoundStatType"></param>
        /// <param name="tick"></param>
        /// <returns></returns>
        public virtual Stat RegisterResourceStat(E statType, float value, E upperBoundStatType, bool tick = true)
        {
            Stat stat = RegisterStat(statType, value, tick);
            if (statType.Equals(upperBoundStatType))
            {
                Debug.LogError("Cannot assign a stat as its own upper bound.");
                return stat;
            }
            if (TryGet(upperBoundStatType, out Stat upperBoundStat))
            {
                stat.SetUpperBound(upperBoundStat);
            }
            else
            {
                Debug.LogError("Upper bound stat not found wile registering resource stat." +
                    "\nMake sure to register upper bound stat before registering the resource stat.");
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