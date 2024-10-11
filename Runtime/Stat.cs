using PlasticGui.WorkspaceWindow.IssueTrackers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        public BindableFloat Base => _base;

        private float _value = 0f;
        private bool _isDirty = false;
        private BindableFloat _base;
        private List<Modifier> _mods = new List<Modifier>();

        public Stat(float value) : this(new BindableFloat(value))
        {

        }

        public Stat(BindableFloat @base)
        {
            _base = @base;
            _base.OnValueChanged += HandleBaseValueChange;
            SetDirty();
        }

        #region API

        public void AddModifier(Modifier mod)
        {
            if (ReferenceEquals(mod.Base, this))
            {
                throw new Exception("Cannot add modifier - Circular base value dependency.");
            }
            _mods.Add(mod);
            mod.Activate();
            mod.OnValueChanged += HandleModifierValueChange;
            SetDirty();
        }

        public void RemoveModifier(Modifier mod)
        {
            mod.Deactivate();
            mod.OnValueChanged -= HandleModifierValueChange;
            SetDirty();
        }

        public void RemoveAllModifiers()
        {
            for(int i = _mods.Count - 1; i >= 0; i--)
            {
                var mod = _mods[i];
                mod.OnValueChanged -= HandleModifierValueChange;
                mod.Deactivate();
            }
            _mods.Clear();
            SetDirty();
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
            if (_mods.Count == 0) return _base.Value;
            float value = _base.Value;
            for (int i = _mods.Count - 1; i >= 0; i--)
            {
                var mod = _mods[i];
                if (mod.IsActive == false)
                {
                    _mods.Remove(mod);
                    continue;
                }
                value += _mods[i].Value;
            }
            return value;
        }
    }
}