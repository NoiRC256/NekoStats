using System;
using System.Collections.Generic;
using NekoLib.ReactiveProps;
using UnityEngine;

namespace NekoLib.Stats
{
    public enum StatChangeMonitorMode
    {
        EveryTick = 0,
        EveryChange = 1,
    }

    /// <summary>
    /// A class representing a stat value. Maintains a collection of modifiers
    /// which contribute to the final value.
    /// <para>The final value is lazily re-calculated on access.</para>
    /// <para>Listen to <see cref="BindableProp{T}.ValueChanged"/> to observe final value.</para>
    /// <para>Listen to <see cref="StatChanged"/> to observe stat instance.</para>
    /// </summary>
    [System.Serializable]
    public class Stat : BindableProp<float>
    {
        #region Inspector Fields

        // Values.
        [Tooltip("Base value of the stat.")]
        [SerializeField] protected float _baseValue = 0f;
        [SerializeField] protected float _offsetValue = 0f;
        [SerializeField] protected float _initialValue = 0f;

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
        [SerializeField] protected bool _enableBroadcast = true;
        [SerializeField] protected StatChangeMonitorMode _changeMonitorMode = StatChangeMonitorMode.EveryTick;
        [SerializeField] protected bool _isDirty = true;
        [SerializeField] protected bool _broadcastChangeThisTick = true;

        #endregion

        #region Properties

        /// <summary>
        /// The final value of the stat.
        /// Lazily calculated on access if marked dirty.
        /// <para>Modifying this will mark the stat dirty for value re-calculation.</para>
        /// </summary>
        public override float Value {
            get {
                if (_isDirty) RefreshValue();
                return _value;
            }
            set {
                _offsetValue += value - _value;
                SetDirty();
            }
        }
        /// <summary>
        /// The base value of the stat.
        /// <para>Modifying this will mark the stat dirty for value re-calculation.</para>
        /// </summary>
        public float BaseValue {
            get => _baseValue;
            set {
                if (value != _baseValue) SetDirty();
                _baseValue = value;
            }
        }

        public float InitialValue {
            get => _initialValue;
            set => _initialValue = value;
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

        #endregion

        #region Events

        /// <summary>
        /// When the stat has been marked dirty for value re-calculation.
        /// The base value or final value may have changed,
        /// or stat modifiers may have been added or removed.
        /// </summary>
        public event Action<Stat> StatChanged;

        #endregion

        public Stat() : this(0f) { }

        public Stat(float baseValue) : base(baseValue)
        {
            _baseValue = baseValue;
            _initialValue = baseValue;
            _useUpperBound = false;
            _useLowerBound = false;
            _upperBound = null;
            _lowerBound = null;
            _hasUpperBound = false;
            _hasLowerBound = false;
            _modifiers = new List<StatModifier>();
            _isDirty = true;
            _broadcastChangeThisTick = true;
        }

        #region Life cycle

        /// <summary>
        /// Clear all variables.
        /// </summary>
        public void Clear()
        {
            _offsetValue = 0f;
            _baseValue = 0f;
            _initialValue = 0f;
            _useUpperBound = false;
            _useLowerBound = false;
            _upperBound = null;
            _lowerBound = null;
            _hasUpperBound = false;
            _hasLowerBound = false;
            _modifiers.Clear();
            _isDirty = true;
            _broadcastChangeThisTick = true;
            StatChanged = null;
        }

        /// <summary>
        /// Reset the stat to its initial state.
        /// </summary>
        /// <param name="clearModifiers"></param>
        public void Reset(bool clearModifiers = false)
        {
            _offsetValue = 0f;
            _baseValue = _initialValue;
            if (clearModifiers) _modifiers.Clear();
            SetDirty();
        }

        #endregion

        #region Modifiers

        /// <summary>
        /// Add a modifier to the stat.
        /// <para>Marks the stat dirty.</para>
        /// </summary>
        /// <param name="modifier"></param>
        public Stat AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
            SetDirty();
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
            SetDirty();
            return this;
        }

        #endregion

        #region Bounds

        public Stat UseUpperBound(bool toggle = true)
        {
            _useUpperBound = toggle;
            return this;
        }

        public Stat UseLowerBound(bool toggle = true)
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
            if (_upperBound != null) _upperBound.ValueChanged -= HandleUpperBoundChanged;
            _upperBound = bindableFloat;
            _upperBound.ValueChanged += HandleUpperBoundChanged;
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
            if (_upperBound != null) _upperBound.ValueChanged -= HandleUpperBoundChanged;
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
            if (_lowerBound != null) _lowerBound.ValueChanged -= HandleLowerBoundChanged;
            _lowerBound = bindableFloat;
            _lowerBound.ValueChanged += HandleLowerBoundChanged;
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
            if (_lowerBound != null) _lowerBound.ValueChanged -= HandleLowerBoundChanged;
            _lowerBound = null;
            _hasLowerBound = false;
            return this;
        }

        private void HandleUpperBoundChanged(float value) { if (_useUpperBound) SetDirty(); }
        private void HandleLowerBoundChanged(float value) { if (_useLowerBound) SetDirty(); }

        #endregion

        #region Value Calculation

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
            float finalValue = _baseValue + (_baseValue * multModifiersValue) + addModifiersValue + _offsetValue;
            if (_useUpperBound && _hasUpperBound && finalValue > _upperBound.Value) finalValue = _upperBound.Value;
            if (_useLowerBound && _hasLowerBound && finalValue < _lowerBound.Value) finalValue = _lowerBound.Value;
            return finalValue;
        }

        #endregion

        #region Value Change Broadcast

        /// <summary>
        /// Check and broadcast any value change since last tick.
        /// <para>Call this at most once per frame for optimal performance.</para>
        /// </summary>
        public void Tick()
        {
            if (_enableBroadcast && _changeMonitorMode == StatChangeMonitorMode.EveryTick)
            {
                // If marked dirty for re-calculation, invoke value change events.
                if (_broadcastChangeThisTick)
                {
                    OnValueChanged();
                }
                _broadcastChangeThisTick = false;
            }
        }

        public Stat EnableBroadcast(bool toggle = true)
        {
            _enableBroadcast = toggle;
            return this;
        }

        public Stat SetChangeMonitorMode(StatChangeMonitorMode changeMonitorMode = StatChangeMonitorMode.EveryTick)
        {
            _changeMonitorMode = changeMonitorMode;
            return this;
        }

        protected override void OnValueChanged()
        {
            if (!_enableBroadcast) return;
            base.OnValueChanged();
            StatChanged?.Invoke(this);
        }

        /// <summary>
        /// Set the stat as dirty.
        /// <para>If <see cref="_changeMonitorMode"/> is <see cref="StatChangeMonitorMode.EveryTick"/>,
        /// will broadcast value change next time <see cref="Tick"/> is executed.</para>
        /// <para>If <see cref="_changeMonitorMode"/> is <see cref="StatChangeMonitorMode.EveryChange"/>,
        /// will broadcast value change right away.</para>
        /// </summary>
        protected void SetDirty()
        {
            _isDirty = true;
            switch (_changeMonitorMode)
            {
                case StatChangeMonitorMode.EveryTick:
                    _broadcastChangeThisTick = true;
                    break;
                case StatChangeMonitorMode.EveryChange:
                    OnValueChanged();
                    break;
            }
        }

        /// <summary>
        /// Re-calculate the final value.
        /// </summary>
        protected void RefreshValue()
        {
            _value = CalculateValue();
            _isDirty = false;
        }

        #endregion

    }
}