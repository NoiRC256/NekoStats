using System;
using System.Collections.Generic;
using CC.Reactive;

namespace CC.Stats
{
    /// <summary>
    /// A class representing a stat value. Maintains a collection of modifiers
    /// which contribute to the final value.
    /// <para>The final value is lazily re-calculated on access.</para>
    /// </summary>
    [Serializable]
    public class Stat : BindableFloatRelay
    {
        private bool _isDirty = false;
        private List<BindableFloatRelay> _modifiers = new List<BindableFloatRelay>();

        public Stat(float value) : base(value)
        {
            _dependency = new BindableFloat(value);
        }

        public Stat(BindableFloat dependency) : base(0f)
        {
            _dependency = dependency;
            Value = dependency.Value;
        }

        #region API

        public void Update()
        {
            if(_isDirty)
            {
                RefreshValue();
                _isDirty = false;
            }
        }

        public void SetDirty()
        {
            _isDirty = true;
        }

        public void AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
            modifier.OnValueChanged += HandleModifierValueChange;
            SetDirty();
        }

        public void RemoveModifier(StatModifier modifier)
        {
            _modifiers.Remove(modifier);
            modifier.OnValueChanged -= HandleModifierValueChange;
            SetDirty();
        }

        #endregion

        protected override void HandleDependencyValueChange(float value)
        {
            SetDirty();
        }

        private void HandleModifierValueChange(float value)
        {
            SetDirty();
        }

        protected override void RefreshValue()
        {
            if (_isDirty)
            {
                Value = CalculateValue();
            }
            else
            {
                SetDirty();
            }
        }

        protected override float CalculateValue()
        {
            float value = _dependency.Value;
            if (_modifiers.Count == 0) return value;
            for (int i = 0; i < _modifiers.Count; i++)
            {
                value += _modifiers[i].Value;
            }
            return value;
        }
    }
}