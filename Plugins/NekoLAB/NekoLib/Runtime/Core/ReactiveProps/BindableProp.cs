using System;
using UnityEngine;

namespace CC.Reactive
{
    [Serializable]
    public class BindableProp<TData> : BindableProp<TData, BindableProp<TData>>
        where TData : struct
    {
        public BindableProp() :base() { }
        public BindableProp(TData value) : base(value) { }
    }

    public abstract class BindableProp<TData, TSelf>
        where TData : struct
        where TSelf : BindableProp<TData, TSelf>
    {
        [SerializeField] protected TData _value;

        public virtual TData Value {
            get => _value;
            set {
                if (!_value.Equals(value))
                {
                    _value = value;
                    BroadcastValueChange();
                }
            }
        }

        /// <summary>
        /// When the value has changed.
        /// </summary>
        public event Action<TSelf> OnValueChanged = delegate { };

        public BindableProp()
        {

        }

        public BindableProp(TData value)
        {
            _value = value;
        }

        protected virtual void BroadcastValueChange()
        {
            OnValueChanged.Invoke((TSelf)this);
        }
    }
}