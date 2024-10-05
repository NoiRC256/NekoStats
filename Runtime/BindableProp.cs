using System;
using UnityEngine;

namespace CC.Stats
{
    public interface IBindableProp<TData> where TData : struct, IEquatable<TData>
    {
        public TData Value { get; }
        public event Action<TData> OnValueChanged;
    }

    [Serializable]
    public class BindableFloat: IBindableProp<float>
    {
        [SerializeField] private float _value;

        public virtual float Value {
            get => _value;
            set {
                if(value != _value)
                {
                    _value = value;
                    BroadcastValueChange(); 
                }
            }
        }

        public event Action<float> OnValueChanged = null;

        public BindableFloat(float value)
        {
            OnValueChanged = delegate { };
            Value = value;
        }

        protected virtual void BroadcastValueChange()
        {
            OnValueChanged.Invoke(_value);
        }
    }
}