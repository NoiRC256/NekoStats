using System;
using System.Collections.Generic;

namespace CC.Stats
{
    /// <summary>
    /// A class representing a stat value. Maintains a collection of modifiers
    /// which contribute to the final value.
    /// <para>The final value is lazily re-calculated on access.</para>
    /// </summary>
    [Serializable]
    public class Stat
    {
        /// <summary>
        /// Final value of the stat, calculated by base value + modifers.
        /// <para>Re-calculates the value if the stat has been marked dirty.</para>
        /// </summary>
        public float Value {
            get {
                if (_isDirty)
                {
                    _value = CalculateValue();
                    _isDirty = false;
                }
                return _value;
            }
        }

        public BindableFloat BaseValue => _baseValue;

        private float _value = 0f;
        private bool _isDirty = false;
        private BindableFloat _baseValue;
        private List<BindableFloat> _modifiers = new List<BindableFloat>();

        public Stat(float value) : this(new BindableFloat(value))
        {

        }

        public Stat(BindableFloat baseValue)
        {
            _baseValue = baseValue;
            _baseValue.OnValueChanged += HandleBaseValueChange;
            SetDirty();
        }

        #region API

        public void AddModifier(Modifier modifier)
        {
            if (ReferenceEquals(modifier.BaseValue, this))
            {
                return;
            }
            _modifiers.Add(modifier);
            modifier.OnValueChanged += HandleModifierValueChange;
            SetDirty();
        }

        public void RemoveModifier(Modifier modifier)
        {
            bool removed = _modifiers.Remove(modifier);
            if (!removed) return;
            modifier.OnValueChanged -= HandleModifierValueChange;
            SetDirty();
        }

        public void SetBaseValue(float value)
        {
            _baseValue.Value = value;
        }

        public void SetDirty()
        {
            _isDirty = true;
        }

        #endregion

        protected void HandleBaseValueChange(float value)
        {
            SetDirty();
        }

        private void HandleModifierValueChange(float value)
        {
            SetDirty();
        }

        private float CalculateValue()
        {
            if (_modifiers.Count == 0) return _baseValue.Value;
            float value = _baseValue.Value;
            for (int i = 0; i < _modifiers.Count; i++)
            {
                value += _modifiers[i].Value;
            }
            return value;
        }
    }
}