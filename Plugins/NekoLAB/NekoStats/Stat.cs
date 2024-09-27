using System;
using System.Collections.Generic;
using CC.Reactive;
using UnityEngine;

namespace CC.Stats
{
    /// <summary>
    /// A class representing a stat value. Maintains a collection of modifiers
    /// which contribute to the final value.
    /// <para>The final value is lazily re-calculated on access.</para>
    /// </summary>
    [Serializable]
    public class Stat : BindableProp<float, Stat>
    {
        #region Variables

        #region Inspector Fields

        // Values.
        [Tooltip("Base value of the stat.")]
        [SerializeField] protected float _valueBase = 0f;
        [SerializeField] protected float _valueOffset = 0f;
        [SerializeField] protected float _valueInitial = 0f;

        // Bounds.
        [Tooltip("If true, will limit the final value by an upper bound if present.")]
        [SerializeField] protected bool _useUpperBound = false;
        [Tooltip("If true, will limit the final value by a lower bound if present.")]
        [SerializeField] protected bool _useLowerBound = false;
        [SerializeField] protected bool _hasUpperBound = false;
        [SerializeField] protected bool _hasLowerBound = false;
        [SerializeField] protected BindableProp<float> _upperBound = null;
        [SerializeField] protected BindableProp<float> _lowerBound = null;

        // Modifiers.
        [SerializeField] protected List<StatModifier> _modifiers = new List<StatModifier>();

        // Value change monitoring.
        [SerializeField] protected bool _isDirty = true;

        #endregion

        #region Properties

        /// <summary>
        /// The final value of the stat.
        /// Lazily calculated on access if marked dirty.
        /// <para>Modifying this will mark the stat dirty for value re-calculation.</para>
        /// </summary>
        public override float Value {
            get {
                if (_isDirty) RecalculateValue();
                return _value;
            }
            set {
                _valueOffset += value - _value;
                BroadcastValueChange();
            }
        }
        /// <summary>
        /// The base value of the stat.
        /// <para>Modifying this will mark the stat dirty for value re-calculation.</para>
        /// </summary>
        public float ValueBase {
            get => _valueBase;
            set {
                if (value != _valueBase) BroadcastValueChange();
                _valueBase = value;
            }
        }

        public float InitialValue {
            get => _valueInitial;
            set => _valueInitial = value;
        }

        /// <summary>
        /// If true, will use both upper bound and lower bounds in final value calculation.
        /// </summary>
        public bool UseBounds {
            get => _useUpperBound && _useLowerBound;
            set {
                _useUpperBound = value; _useLowerBound = value;
            }
        }
        public bool HasUpperBound => _hasUpperBound;
        public bool HasLowerBound => _hasLowerBound;
        public BindableProp<float> UpperBound => _upperBound;
        public BindableProp<float> LowerBound => _lowerBound;

        #endregion

        #endregion

        public Stat() : this(0f) { }

        public Stat(float baseValue) : base(baseValue)
        {
            _valueBase = baseValue;
            _valueInitial = baseValue;
            _useUpperBound = false;
            _useLowerBound = false;
            _upperBound = null;
            _lowerBound = null;
            _hasUpperBound = false;
            _hasLowerBound = false;
            _modifiers = new List<StatModifier>();
            _isDirty = true;
        }

        #region API

        public void Tick()
        {

        }

        #region Life Cycle API

        /// <summary>
        /// Clear all variables.
        /// </summary>
        public void Clear()
        {
            _valueOffset = 0f;
            _valueBase = 0f;
            _valueInitial = 0f;
            _useUpperBound = false;
            _useLowerBound = false;
            _upperBound = null;
            _lowerBound = null;
            _hasUpperBound = false;
            _hasLowerBound = false;
            _modifiers.Clear();
            _isDirty = true;
        }

        /// <summary>
        /// Reset the stat to its initial state.
        /// </summary>
        /// <param name="clearModifiers"></param>
        public void Reset(bool clearModifiers = false)
        {
            _valueOffset = 0f;
            _valueBase = _valueInitial;
            if (clearModifiers) _modifiers.Clear();
            BroadcastValueChange();
        }

        #endregion

        #region Modifiers API

        /// <summary>
        /// Add a modifier to the stat.
        /// <para>Marks the stat dirty.</para>
        /// </summary>
        /// <param name="modifier"></param>
        public Stat AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
            BroadcastValueChange();
            return this;
        }

        /// <summary>
        /// Remove a modifier from the stat.
        /// <para>Marks the stat dirty.</para>
        /// </summary>
        /// <param name="modifier"></param>
        public Stat RemoveModifier(StatModifier modifier)
        {
            _modifiers.Remove(modifier);
            BroadcastValueChange();
            return this;
        }

        #endregion

        #region Bounds API

        public Stat SetUseUpperBound(bool toggle = true)
        {
            _useUpperBound = toggle;
            return this;
        }

        public Stat SetUseLowerBound(bool toggle = true)
        {
            _useLowerBound = toggle;
            return this;
        }

        /// <summary>
        /// Assign an upper bound to the stat.
        /// </summary>
        /// <param name="bindableFloat"></param>
        /// <returns></returns>
        public Stat SetUpperBound(BindableProp<float> bindableFloat, bool useUpperBound = true)
        {
            if (_upperBound != null) _upperBound.OnValueChanged -= HandleUpperBoundChanged;
            _upperBound = bindableFloat;
            _upperBound.OnValueChanged += HandleUpperBoundChanged;
            _hasUpperBound = true;
            _useUpperBound = useUpperBound;
            return this;
        }

        /// <summary>
        /// Assign an upper bound to the stat.
        /// </summary>
        /// <param name="upperBound"></param>
        /// <returns></returns>
        public Stat SetUpperBound(float upperBound, bool useUpperBound = true)
            => SetUpperBound(new BindableProp<float>(upperBound), useUpperBound);

        /// <summary>
        /// Remove the upper bound of the stat.
        /// </summary>
        public Stat RemoveUpperBound()
        {
            if (_upperBound != null) _upperBound.OnValueChanged -= HandleUpperBoundChanged;
            _upperBound = null;
            _hasUpperBound = false;
            return this;
        }

        /// <summary>
        /// Assign a lower bound to the stat.
        /// </summary>
        /// <param name="bindableFloat"></param>
        /// <returns></returns>
        public Stat SetLowerBound(BindableProp<float> bindableFloat, bool useLowerBound = true)
        {
            if (_lowerBound != null) _lowerBound.OnValueChanged -= HandleLowerBoundChanged;
            _lowerBound = bindableFloat;
            _lowerBound.OnValueChanged += HandleLowerBoundChanged;
            _hasLowerBound = true;
            _useLowerBound = useLowerBound;
            return this;
        }

        /// <summary>
        /// Assign a lower bound to the stat.
        /// </summary>
        /// <param name="lowerBound"></param>
        /// <returns></returns>
        public Stat SetLowerBound(float lowerBound = 0f, bool useLowerBound = true)
            => SetLowerBound(new BindableProp<float>(lowerBound), useLowerBound);

        /// <summary>
        /// Remove the lower bound of the stat.
        /// </summary>
        public Stat RemoveLowerBound()
        {
            if (_lowerBound != null) _lowerBound.OnValueChanged -= HandleLowerBoundChanged;
            _lowerBound = null;
            _hasLowerBound = false;
            return this;
        }

        #endregion

        #endregion

        #region Value Calculation

        /// <summary>
        /// Calculate and update the final value.
        /// </summary>
        protected void RecalculateValue()
        {
            _value = CalculateValue();
            _isDirty = false;
        }

        /// <summary>
        /// Calculate the final value.
        /// </summary>
        /// <returns></returns>
        protected float CalculateValue()
        {
            // Aggregate modifier values.
            float addModifiersValue = 0f;
            float multModifiersValue = 0f;
            for (int i = 0; i < _modifiers.Count; i++)
            {
                StatModifier modifier = _modifiers[i];
                if (modifier.Effect == StatModifierEffect.Add)
                {
                    addModifiersValue += modifier.Value;
                }
                else if (modifier.Effect == StatModifierEffect.Mult)
                {
                    multModifiersValue += modifier.Value;
                }
            }

            // Calculate final value.
            float finalValue = _valueBase + (_valueBase * multModifiersValue) + addModifiersValue + _valueOffset;
            if (_useUpperBound && _hasUpperBound && finalValue > _upperBound.Value) finalValue = _upperBound.Value;
            if (_useLowerBound && _hasLowerBound && finalValue < _lowerBound.Value) finalValue = _lowerBound.Value;
            return finalValue;
        }

        #endregion

        #region Value Change Broadcast

        private void HandleUpperBoundChanged(BindableProp<float> bound)
        {
            if (_useUpperBound) BroadcastValueChange();
        }

        private void HandleLowerBoundChanged(BindableProp<float> bound)
        {
            if (_useLowerBound) BroadcastValueChange();
        }

        protected override void BroadcastValueChange()
        {
            // Next time any subscriber accesses the final value,
            // it will be recalculated.
            _isDirty = true;
            base.BroadcastValueChange();
        }

        #endregion
    }
}