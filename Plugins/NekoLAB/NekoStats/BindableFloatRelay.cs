using CC.Reactive;
using System;

namespace CC.Stats
{
    public abstract class BindableFloatRelay : BindableFloat
    {
        public BindableFloat Dependency {
            get => _dependency;
            protected set {
                if (value != _dependency)
                {
                    if (_dependency != null)
                    {
                        _dependency.OnValueChanged -= HandleDependencyValueChange;
                    }
                    _dependency = value;
                    if (_dependency != null)
                    {
                        _dependency.OnValueChanged += HandleDependencyValueChange;
                    }
                    HandleDependencyValueChange(_dependency.Value);
                }
            }
        }

        protected BindableFloat _dependency = null;

        public BindableFloatRelay(float value) : base(value)
        {
        }

        #region API

        public void SetDependency(BindableFloat dep)
        {
            Dependency = dep;
        }

        public void RemoveDependency()
        {
            Dependency = null;
        }

        #endregion

        protected virtual void HandleDependencyValueChange(float value)
        {
            RefreshValue();
        }

        protected virtual void RefreshValue()
        {
            Value = CalculateValue();
        }

        protected abstract float CalculateValue();
    }

    [Serializable]
    public class StatModifier : BindableFloatRelay
    {
        protected float _modValue;

        public StatModifier(float value) : base(value)
        {
            _modValue = value;
        }

        public void SetModValue(float value)
        {
            _modValue = value;
            RefreshValue();
        }

        protected override float CalculateValue()
        {
            if (_dependency == null) return _modValue;
            return _dependency.Value + _modValue;
        }
    }

    [Serializable]
    public class StatModifierPercent : StatModifier
    {
        public StatModifierPercent(float value, BindableFloat dependency) : base(value)
        {
            _dependency = dependency;
        }

        protected override float CalculateValue()
        {
            return _dependency.Value * _modValue;
        }
    }
}