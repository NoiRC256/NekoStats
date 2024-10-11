using System;

namespace CC.Stats
{
    [Serializable]
    public class Modifier : BindableFloat
    {
        protected float _modValue;
        protected readonly BindableFloat _base = null;

        public BindableFloat Base {
            get => _base;
        }

        public bool IsActive {
            get; private set;
        }

        public Modifier(float modValue) : base(modValue)
        {
            _modValue = modValue;
        }

        public Modifier(float modValue, BindableFloat @base) : this(modValue)
        {
            if (@base == null) return;
            if (ReferenceEquals(@base, this))
            {
                throw new Exception("BindableFloat circular dependency: Modifier base value cannot be self.");
            }
            _base = @base;
            _base.OnValueChanged += HandleBaseValueChange;
            RefreshValue();
        }

        #region API

        public void SetModValue(float value)
        {
            _modValue = value;
            RefreshValue();
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        #endregion

        private void HandleBaseValueChange(float value)
        {
            RefreshValue();
        }

        private void RefreshValue()
        {
            Value = CalculateValue();
        }

        protected virtual float CalculateValue()
        {
            if (_base == null) return _modValue;
            return _base.Value + _modValue;
        }
    }

    [Serializable]
    public class PercentModifier : Modifier
    {
        public PercentModifier(float modValue, BindableFloat @base) : base(modValue, @base)
        {
        }

        protected override float CalculateValue()
        {
            return _base.Value * _modValue;
        }
    }
}