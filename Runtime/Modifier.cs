using System;

namespace CC.Stats
{
    [Serializable]
    public class Modifier : BindableFloat
    {
        protected float _modValue; 
        protected readonly BindableFloat _baseValue = null;

        public BindableFloat BaseValue {
            get => _baseValue;
        }

        public Modifier(float modValue) : base(modValue)
        {
            _modValue = modValue;
        }

        public Modifier(float modValue, BindableFloat baseValue) : this(modValue)
        {
            if(ReferenceEquals(baseValue, this))
            {
                throw new Exception("BindableFloat circular dependency: Modifier base value cannot be self.");
            }
            _baseValue = baseValue;
            _baseValue.OnValueChanged += HandleBaseValueChange;
            RefreshValue();
        }

        #region API

        public void SetModValue(float value)
        {
            _modValue = value;
            RefreshValue();
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
            if (_baseValue == null) return _modValue;
            return _baseValue.Value + _modValue;
        }
    }

    [Serializable]
    public class PercentModifier : Modifier
    {
        public PercentModifier(float modValue, BindableFloat baseValue) : base(modValue, baseValue)
        {
        }

        protected override float CalculateValue()
        {
            return _baseValue.Value * _modValue;
        }
    }
}