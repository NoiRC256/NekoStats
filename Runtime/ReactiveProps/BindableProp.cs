using System;
using UnityEngine;

namespace NekoLib.ReactiveProps
{
    /// <summary>
    /// Wrapper class that holds a value and exposes an event for value change.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class BindableProp<T> : IBindableProp<T> where T : struct
    {
        [SerializeField] protected T _value;

        public virtual T Value {
            get => _value;
            set {
                if (!_value.Equals(value))
                {
                    _value = value;
                    OnValueChanged();
                }
            }
        }

        /// <summary>
        /// When the value has changed.
        /// </summary>
        public event Action<T> ValueChanged;

        public BindableProp()
        {

        }

        public BindableProp(T value)
        {
            _value = value;
        }

        protected virtual void OnValueChanged()
        {
            ValueChanged?.Invoke(Value);
        }
    }

    [System.Serializable]
    public class BindableInt : BindableProp<int>
    {
        public BindableInt() : this(0) { }
        public BindableInt(int value) : base(value)
        {
        }
    }

    [System.Serializable]
    public class BindableFloat : BindableProp<float>
    {
        public BindableFloat() : this(0f) { }
        public BindableFloat(float value) : base(value)
        {
        }
    }

    [System.Serializable]
    public class BindableDouble : BindableProp<double>
    {
        public BindableDouble() : this(0d) { }
        public BindableDouble(double value) : base(value)
        {
        }
    }

    [System.Serializable]
    public class BindableBool : BindableProp<bool>
    {
        public BindableBool() : this(false) { }
        public BindableBool(bool value) : base(value)
        {
        }
    }

    [System.Serializable]
    public class BindbableVector2 : BindableProp<Vector2>
    {
        public BindbableVector2() : this(Vector2.zero) { }
        public BindbableVector2(Vector2 value) : base(value)
        {
        }
    }

    [System.Serializable]
    public class BindbableVector3 : BindableProp<Vector3>
    {
        public BindbableVector3() : this(Vector3.zero) { }
        public BindbableVector3(Vector3 value) : base(value)
        {
        }
    }
}